
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnASP.Models;

public class SanphamsController : Controller
{
    private readonly QlquanaoContext _context;

    public SanphamsController(QlquanaoContext context)
    {
        _context = context;
    }

    // GET: SANPHAMS
    public async Task<IActionResult> Index(string searchString, string loaiSp, string sort, int? page)
    {
        var sanphams = _context.Sanphams.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            sanphams = sanphams.Where(s => s.TenSp.Contains(searchString) || (s.MoTa != null && s.MoTa.Contains(searchString)));
        }

        if (!string.IsNullOrEmpty(loaiSp))
        {
            sanphams = sanphams.Where(s => s.LoaiSp == loaiSp);
        }

        // Sắp xếp
        sanphams = sort switch
        {
            "price_asc" => sanphams.OrderBy(s => s.Gia),
            "price_desc" => sanphams.OrderByDescending(s => s.Gia),
            _ => sanphams.OrderByDescending(s => s.NgayDang) // Mặc định: mới nhất
        };

        ViewBag.SearchString = searchString;
        ViewBag.LoaiSp = loaiSp;
        ViewBag.Sort = sort;
        ViewBag.LoaiSpList = await _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToListAsync();

        int pageSize = 12;
        int pageNumber = page ?? 1;

        var totalItems = await sanphams.CountAsync();
        var items = await sanphams
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.TotalItems = totalItems;

        return View(items);
    }

    // GET: SANPHAMS/Details/5
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

        // Load related products (same category, excluding current)
        var relatedProducts = await _context.Sanphams
            .Where(s => s.LoaiSp == sanpham.LoaiSp && s.Id != sanpham.Id && s.Soluong > 0)
            .OrderByDescending(s => s.NgayDang)
            .Take(4)
            .ToListAsync();

        ViewBag.RelatedProducts = relatedProducts;

        return View(sanpham);
    }

    // GET: Sanphams/GetSoldCount/5
    public async Task<IActionResult> GetSoldCount(int id)
    {
        var soldCount = await _context.Chitietdonhangs
            .Where(c => c.IdSanpham == id)
            .Join(_context.Donhangs.Where(d => d.TrangThai != "huy"),
                  c => c.IdDonhang,
                  d => d.Id,
                  (c, d) => c.SoLuong)
            .SumAsync();

        return Json(new { count = soldCount });
    }

    // GET: SANPHAMS/Create
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: SANPHAMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType,Chitietdonhangs")] Sanpham sanpham)
    {
        if (ModelState.IsValid)
        {
            _context.Add(sanpham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(sanpham);
    }

    // GET: SANPHAMS/Edit/5
    [Authorize(Roles = "admin")]
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
        return View(sanpham);
    }

    // POST: SANPHAMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType,Chitietdonhangs")] Sanpham sanpham)
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
        return View(sanpham);
    }

    // GET: SANPHAMS/Delete/5
    [Authorize(Roles = "admin")]
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

    // POST: SANPHAMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var sanpham = await _context.Sanphams.FindAsync(id);
        if (sanpham != null)
        {
            _context.Sanphams.Remove(sanpham);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SanphamExists(int? id)
    {
        return _context.Sanphams.Any(e => e.Id == id);
    }
}
