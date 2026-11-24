using DoAnASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class DonhangsController : Controller
    {
        private readonly QlquanaoContext _context;

        public DonhangsController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Admin/Donhangs
        public async Task<IActionResult> Index(string status, int? page)
        {
            var donhangs = _context.Donhangs
                .Include(d => d.IdNguoidatNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                donhangs = donhangs.Where(d => d.TrangThai == status);
            }

            ViewBag.Status = status;
            ViewBag.StatusList = new List<string> { "dang_xu_ly", "da_giao", "huy" };

            int pageSize = 15;
            int pageNumber = page ?? 1;

            return View(await PaginatedList<Donhang>.CreateAsync(donhangs.OrderByDescending(d => d.NgayDat), pageNumber, pageSize));
        }

        // GET: Admin/Donhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donhang = await _context.Donhangs
                .Include(d => d.IdNguoidatNavigation)
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(ct => ct.IdSanphamNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (donhang == null)
            {
                return NotFound();
            }

            return View(donhang);
        }

        // GET: Admin/Donhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donhang = await _context.Donhangs.FindAsync(id);
            if (donhang == null)
            {
                return NotFound();
            }

            ViewBag.StatusList = GetAllowedStatusTransitions(donhang.TrangThai);

            return View(donhang);
        }

        // POST: Admin/Donhangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,IdNguoidat,TongTien,NgayDat,TrangThai,TenNguoinhan,Sdt,DiaChi,PhuongThucTt,GhiChu")] Donhang donhangInput)
        {
            if (id != donhangInput.Id)
            {
                return NotFound();
            }

            var donhang = await _context.Donhangs
                .Include(d => d.Chitietdonhangs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donhang == null)
            {
                return NotFound();
            }

            var oldStatus = donhang.TrangThai;
            var newStatus = donhangInput.TrangThai;

            // Validate status transition
            var allowedTransitions = GetAllowedStatusTransitions(oldStatus);
            if (!allowedTransitions.Any(s => s.Value == newStatus))
            {
                ModelState.AddModelError("TrangThai", $"Không thể chuyển từ '{GetStatusText(oldStatus)}' sang '{GetStatusText(newStatus)}'.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Update other fields
                    donhang.TenNguoinhan = donhangInput.TenNguoinhan;
                    donhang.Sdt = donhangInput.Sdt;
                    donhang.DiaChi = donhangInput.DiaChi;
                    donhang.PhuongThucTt = donhangInput.PhuongThucTt;
                    donhang.GhiChu = donhangInput.GhiChu;

                    // Handle status change with stock restoration
                    if (oldStatus != newStatus)
                    {
                        await HandleStatusChange(donhang, oldStatus, newStatus);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Cáº­p nháº­t đơn hÃ ng thÃ nh công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                }
            }

            ViewBag.StatusList = GetAllowedStatusTransitions(donhang.TrangThai);
            return View(donhang);
        }

        private List<SelectListItem> GetAllowedStatusTransitions(string currentStatus)
        {
            // Valid transitions:
            // dang_xu_ly -> da_giao, huy
            // da_giao -> (no further transitions)
            // huy -> (no further transitions)
            return currentStatus switch
            {
                "dang_xu_ly" => new List<SelectListItem>
                {
                    new SelectListItem { Value = "dang_xu_ly", Text = "Đang xá»­ lý", Selected = true },
                    new SelectListItem { Value = "da_giao", Text = "\u0110\u00e3 giao" },
                    new SelectListItem { Value = "huy", Text = "Đã hủy" }
                },
                "da_giao" => new List<SelectListItem>
                {
                    new SelectListItem { Value = "da_giao", Text = "Đã giao", Selected = true }
                },
                "huy" => new List<SelectListItem>
                {
                    new SelectListItem { Value = "huy", Text = "Đã hủy", Selected = true }
                },
                _ => new List<SelectListItem>
                {
                    new SelectListItem { Value = currentStatus, Text = GetStatusText(currentStatus), Selected = true }
                }
            };
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "dang_xu_ly" => "Đang xá»­ lý",
                "da_giao" => "Đã giao",
                "huy" => "Đã hủy",
                _ => status
            };
        }

        private async Task HandleStatusChange(Donhang donhang, string oldStatus, string newStatus)
        {
            // If changing TO "huy" (cancelled), restore stock
            if (newStatus == "huy" && oldStatus != "huy")
            {
                foreach (var chiTiet in donhang.Chitietdonhangs)
                {
                    var product = await _context.Sanphams.FindAsync(chiTiet.IdSanpham);
                    if (product != null)
                    {
                        product.Soluong += chiTiet.SoLuong;
                        _context.Sanphams.Update(product);
                    }
                }
            }
            // If changing FROM "huy" to something else (shouldn't happen due to validation), deduct stock again
            // This is blocked by validation, but just in case
            else if (oldStatus == "huy" && newStatus != "huy")
            {
                foreach (var chiTiet in donhang.Chitietdonhangs)
                {
                    var product = await _context.Sanphams.FindAsync(chiTiet.IdSanpham);
                    if (product != null)
                    {
                        if (product.Soluong < chiTiet.SoLuong)
                        {
                            throw new InvalidOperationException($"Không đủ tồn kho cho sản phẩm {product.TenSp} để chuyển trạng thái.");
                        }
                        product.Soluong -= chiTiet.SoLuong;
                        _context.Sanphams.Update(product);
                    }
                }
            }
        }

        private bool DonhangExists(int? id)
        {
            return _context.Donhangs.Any(e => e.Id == id);
        }
    }
}
