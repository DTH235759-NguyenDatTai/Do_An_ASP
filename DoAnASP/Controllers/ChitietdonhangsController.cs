
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnASP.Models;

public class ChitietdonhangsController : Controller
{
    private readonly QlquanaoContext _context;

    public ChitietdonhangsController(QlquanaoContext context)
    {
        _context = context;
    }

    // GET: CHITIETDONHANGS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Chitietdonhangs.ToListAsync());
    }

    // GET: CHITIETDONHANGS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chitietdonhang = await _context.Chitietdonhangs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (chitietdonhang == null)
        {
            return NotFound();
        }

        return View(chitietdonhang);
    }

    // GET: CHITIETDONHANGS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CHITIETDONHANGS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,IdDonhang,IdSanpham,SoLuong,DonGia,IdDonhangNavigation,IdSanphamNavigation")] Chitietdonhang chitietdonhang)
    {
        if (ModelState.IsValid)
        {
            _context.Add(chitietdonhang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(chitietdonhang);
    }

    // GET: CHITIETDONHANGS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chitietdonhang = await _context.Chitietdonhangs.FindAsync(id);
        if (chitietdonhang == null)
        {
            return NotFound();
        }
        return View(chitietdonhang);
    }

    // POST: CHITIETDONHANGS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,IdDonhang,IdSanpham,SoLuong,DonGia,IdDonhangNavigation,IdSanphamNavigation")] Chitietdonhang chitietdonhang)
    {
        if (id != chitietdonhang.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(chitietdonhang);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChitietdonhangExists(chitietdonhang.Id))
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
        return View(chitietdonhang);
    }

    // GET: CHITIETDONHANGS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chitietdonhang = await _context.Chitietdonhangs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (chitietdonhang == null)
        {
            return NotFound();
        }

        return View(chitietdonhang);
    }

    // POST: CHITIETDONHANGS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var chitietdonhang = await _context.Chitietdonhangs.FindAsync(id);
        if (chitietdonhang != null)
        {
            _context.Chitietdonhangs.Remove(chitietdonhang);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ChitietdonhangExists(int? id)
    {
        return _context.Chitietdonhangs.Any(e => e.Id == id);
    }
}
