using DoAnASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
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
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Email,Password,Role,Sdt,DiaChi,NgayTao")] Taikhoan khachhang)
        {
            if (id != khachhang.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        private bool KhachhangExists(int? id)
        {
            return _context.Taikhoans.Any(e => e.Id == id);
        }
    }
}