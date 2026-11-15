
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnASP.Models;

public class DonhangsController : Controller
{
    private readonly QlquanaoContext _context;

    public DonhangsController(QlquanaoContext context)
    {
        _context = context;
    }

    // GET: DONHANGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Donhangs.ToListAsync());
    }

    // GET: DONHANGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var donhang = await _context.Donhangs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (donhang == null)
        {
            return NotFound();
        }

        return View(donhang);
    }

    // GET: DONHANGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: DONHANGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,IdNguoidat,TongTien,NgayDat,TrangThai,TenNguoinhan,Sdt,DiaChi,PhuongThucTt,GhiChu,Chitietdonhangs,IdNguoidatNavigation")] Donhang donhang)
    {
        if (ModelState.IsValid)
        {
            _context.Add(donhang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(donhang);
    }

    // GET: DONHANGS/Edit/5
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
        return View(donhang);
    }

    // POST: DONHANGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,IdNguoidat,TongTien,NgayDat,TrangThai,TenNguoinhan,Sdt,DiaChi,PhuongThucTt,GhiChu,Chitietdonhangs,IdNguoidatNavigation")] Donhang donhang)
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
        return View(donhang);
    }

    // GET: DONHANGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var donhang = await _context.Donhangs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (donhang == null)
        {
            return NotFound();
        }

        return View(donhang);
    }

    // POST: DONHANGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var donhang = await _context.Donhangs.FindAsync(id);
        if (donhang != null)
        {
            _context.Donhangs.Remove(donhang);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DonhangExists(int? id)
    {
        return _context.Donhangs.Any(e => e.Id == id);
    }
}
