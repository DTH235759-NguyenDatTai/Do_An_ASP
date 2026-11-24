using System.Security.Claims;
using DoAnASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly QlquanaoContext _context;
        private const string CartSessionKey = "Cart";

        public OrdersController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống, không thể đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Taikhoans.FindAsync(userId);

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                TenNguoiNhan = user?.Name ?? "",
                Sdt = user?.Sdt ?? "",
                DiaChi = user?.DiaChi ?? "",
                PhuongThucThanhToan = "cod"
            };

            return View(model);
        }

        // POST: Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống, không thể đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Validate input
            if (string.IsNullOrWhiteSpace(model.TenNguoiNhan))
                ModelState.AddModelError(nameof(model.TenNguoiNhan), "Tên người nhận không được để trống.");
            if (string.IsNullOrWhiteSpace(model.Sdt))
                ModelState.AddModelError(nameof(model.Sdt), "Số điện thoại không được để trống.");
            if (string.IsNullOrWhiteSpace(model.DiaChi))
                ModelState.AddModelError(nameof(model.DiaChi), "Địa chỉ không được để trống.");

            if (model.PhuongThucThanhToan is not ("cod" or "chuyen_khoan"))
                ModelState.AddModelError(nameof(model.PhuongThucThanhToan), "Invalid payment method.");

            // Re-read products from DB to get current price and stock
            var productIds = cart.Select(c => c.ProductId).ToList();
            var products = await _context.Sanphams
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Check stock and calculate total
            long tongTien = 0;
            foreach (var item in cart)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    ModelState.AddModelError("", $"Sản phẩm {item.ProductName} không còn tồn tại.");
                    continue;
                }

                if (product.Soluong < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm {product.TenSp} chỉ còn {product.Soluong} trong kho.");
                    continue;
                }

                // Use DB price at time of order
                item.Price = product.Gia;
                tongTien += (long)(item.Price * item.Quantity);
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                return View(model);
            }

            // Create order in transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var donhang = new Donhang
                {
                    IdNguoidat = userId,
                    TongTien = tongTien,
                    NgayDat = DateTime.Now,
                    TrangThai = "dang_xu_ly",
                    TenNguoinhan = model.TenNguoiNhan.Trim(),
                    Sdt = model.Sdt.Trim(),
                    DiaChi = model.DiaChi.Trim(),
                    PhuongThucTt = model.PhuongThucThanhToan,
                    GhiChu = model.GhiChu?.Trim()
                };

                _context.Donhangs.Add(donhang);
                await _context.SaveChangesAsync(); // Get donhang.Id

                // Create order details and deduct stock
                foreach (var item in cart)
                {
                    var product = products[item.ProductId];

                    var chiTiet = new Chitietdonhang
                    {
                        IdDonhang = donhang.Id,
                        IdSanpham = item.ProductId,
                        SoLuong = item.Quantity,
                        DonGia = item.Price
                    };
                    _context.Chitietdonhangs.Add(chiTiet);

                    // Deduct stock
                    product.Soluong -= item.Quantity;
                    _context.Sanphams.Update(product);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Clear cart
                ClearCart();

                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("Success", new { id = donhang.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Đã có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại.");
                model.CartItems = cart;
                return View(model);
            }
        }

        // GET: Orders/Success/5
        public async Task<IActionResult> Success(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var donhang = await _context.Donhangs
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(c => c.IdSanphamNavigation)
                .FirstOrDefaultAsync(d => d.Id == id && d.IdNguoidat == userId);

            if (donhang == null)
                return NotFound();

            return View(donhang);
        }

        // GET: Orders/MyOrders
        public async Task<IActionResult> MyOrders(int? page)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Donhangs
                .Where(d => d.IdNguoidat == userId)
                .OrderByDescending(d => d.NgayDat)
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(c => c.IdSanphamNavigation);

            var totalItems = await query.CountAsync();
            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var donhang = await _context.Donhangs
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(c => c.IdSanphamNavigation)
                .FirstOrDefaultAsync(d => d.Id == id && d.IdNguoidat == userId);

            if (donhang == null)
                return NotFound();

            return View(donhang);
        }

        // POST: Orders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var donhang = await _context.Donhangs
                .Include(d => d.Chitietdonhangs)
                .FirstOrDefaultAsync(d => d.Id == id && d.IdNguoidat == userId);

            if (donhang == null)
                return NotFound();

            if (donhang.TrangThai != "dang_xu_ly")
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng ở trạng thái 'Đang xử lý'.";
                return RedirectToAction("Details", new { id });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Restore stock
                foreach (var chiTiet in donhang.Chitietdonhangs)
                {
                    var product = await _context.Sanphams.FindAsync(chiTiet.IdSanpham);
                    if (product != null)
                    {
                        product.Soluong += chiTiet.SoLuong;
                        _context.Sanphams.Update(product);
                    }
                }

                donhang.TrangThai = "huy";
                _context.Donhangs.Update(donhang);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Hủy đơn hàng thành công, tồn kho đã được hoàn lại.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra khi hủy đơn hàng.";
            }

            return RedirectToAction("Details", new { id });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }
    }

    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public string TenNguoiNhan { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string PhuongThucThanhToan { get; set; } = "cod";
        public string? GhiChu { get; set; }
    }
}