using DoAnASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class KhachhangsController : Controller
    {
        private readonly QlquanaoContext _context;

        public KhachhangsController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Admin/Khachhangs
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var khachhangs = _context.Taikhoans.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                khachhangs = khachhangs.Where(k => k.Name.Contains(searchString) || k.Email.Contains(searchString) || (k.Sdt != null && k.Sdt.Contains(searchString)));
            }

            ViewBag.SearchString = searchString;

            int pageSize = 15;
            int pageNumber = page ?? 1;

            return View(await PaginatedList<Taikhoan>.CreateAsync(khachhangs.OrderByDescending(k => k.NgayTao), pageNumber, pageSize));
        }

        // GET: Admin/Khachhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Taikhoans
                .Include(k => k.Donhangs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khachhang == null)
            {
                return NotFound();
            }

            // Thống kê đơn hàng của khách
            ViewBag.TongDonHang = khachhang.Donhangs.Count;
            ViewBag.TongChiTieu = khachhang.Donhangs.Where(d => d.TrangThai == "da_giao").Sum(d => d.TongTien);
            ViewBag.DonHangGanNhat = khachhang.Donhangs.OrderByDescending(d => d.NgayDat).FirstOrDefault();

            return View(khachhang);
        }

        // GET: Admin/Khachhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachhang = await _context.Taikhoans.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }

            return View(khachhang);
        }

        // POST: Admin/Khachhangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Email,Role,Sdt,DiaChi")] Taikhoan khachhangInput)
        {
            if (id != khachhangInput.Id)
            {
                return NotFound();
            }

            var khachhang = await _context.Taikhoans.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }

            // Không cho admin tự hạ quyền của chính mình
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (khachhang.Id == currentUserId && khachhangInput.Role != khachhang.Role)
            {
                ModelState.AddModelError("Role", "Không thể thay đổi quyền của chính mình.");
            }

            // Kiểm tra email không trùng
            if (await _context.Taikhoans.AnyAsync(t => t.Email == khachhangInput.Email && t.Id != khachhang.Id))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi tài khoản khác.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Chỉ cập nhật các trường cho phép
                    khachhang.Name = khachhangInput.Name;
                    khachhang.Email = khachhangInput.Email;
                    khachhang.Role = khachhangInput.Role;
                    khachhang.Sdt = khachhangInput.Sdt;
                    khachhang.DiaChi = khachhangInput.DiaChi;
                    // KHÔNG cập nhật Password ở đây

                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachhangExists(khachhang.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachhang);
        }

        // POST: Admin/Khachhangs/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var khachhang = await _context.Taikhoans.FindAsync(id);
            if (khachhang == null)
            {
                return NotFound();
            }

            khachhang.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công!";
            return RedirectToAction(nameof(Edit), new { id });
        }

        private bool KhachhangExists(int? id)
        {
            return _context.Taikhoans.Any(e => e.Id == id);
        }
    }
}