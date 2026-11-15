using DoAnASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanphamsController : Controller
    {
        private readonly QlquanaoContext _context;

        public SanphamsController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Admin/Sanphams
        public async Task<IActionResult> Index(string searchString, string loaiSp, int? page)
        {
            var sanphams = _context.Sanphams.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(s => s.TenSp.Contains(searchString) || s.LoaiSp.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(loaiSp))
            {
                sanphams = sanphams.Where(s => s.LoaiSp == loaiSp);
            }

            ViewBag.SearchString = searchString;
            ViewBag.LoaiSp = loaiSp;
            ViewBag.LoaiSpList = await _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToListAsync();

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return View(await PaginatedList<Sanpham>.CreateAsync(sanphams.OrderByDescending(s => s.NgayDang), pageNumber, pageSize));
        }

        // GET: Admin/Sanphams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // GET: Admin/Sanphams/Create
        public IActionResult Create()
        {
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View();
        }

        // POST: Admin/Sanphams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType")] Sanpham sanpham)
        {
            if (ModelState.IsValid)
            {
                sanpham.NgayDang = DateTime.Now;
                _context.Add(sanpham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // GET: Admin/Sanphams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // POST: Admin/Sanphams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType")] Sanpham sanpham)
        {
            if (id != sanpham.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanpham);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanphamExists(sanpham.Id))
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
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // GET: Admin/Sanphams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // POST: Admin/Sanphams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham != null)
            {
                _context.Sanphams.Remove(sanpham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SanphamExists(int? id)
        {
            return _context.Sanphams.Any(e => e.Id == id);
        }
    }

    // Helper class for pagination
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}