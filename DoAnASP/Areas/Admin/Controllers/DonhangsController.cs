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
            ViewBag.StatusList = new List<string> { "dang_xu_ly", "dang_giao", "da_giao", "da_huy" };

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

            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "dang_xu_ly", Text = "Đang xử lý" },
                new SelectListItem { Value = "dang_giao", Text = "Đang giao" },
                new SelectListItem { Value = "da_giao", Text = "Đã giao" },
                new SelectListItem { Value = "da_huy", Text = "Đã hủy" }
            };

            return View(donhang);
        }

        // POST: Admin/Donhangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,IdNguoidat,TongTien,NgayDat,TrangThai,TenNguoinhan,Sdt,DiaChi,PhuongThucTt,GhiChu")] Donhang donhang)
        {
            if (id != donhang.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonhangExists(donhang.Id))
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

            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "dang_xu_ly", Text = "Đang xử lý" },
                new SelectListItem { Value = "dang_giao", Text = "Đang giao" },
                new SelectListItem { Value = "da_giao", Text = "Đã giao" },
                new SelectListItem { Value = "da_huy", Text = "Đã hủy" }
            };

            return View(donhang);
        }

        private bool DonhangExists(int? id)
        {
            return _context.Donhangs.Any(e => e.Id == id);
        }
    }
}