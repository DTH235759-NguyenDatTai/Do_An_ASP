using DoAnASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAnASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly QlquanaoContext _context;

        public HomeController(QlquanaoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Best Sellers - from v_sp_banchay view joined with Sanphams
            var bestSellerIds = await _context.VSpBanchays
                .OrderByDescending(v => v.DaBan)
                .Take(8)
                .Select(v => v.Id)
                .ToListAsync();

            var bestSellerProducts = await _context.Sanphams
                .Where(s => bestSellerIds.Contains(s.Id) && s.Soluong > 0)
                .ToListAsync();
            var bestSellers = bestSellerProducts
                .OrderByDescending(s => bestSellerIds.IndexOf(s.Id))
                .ToList();

            // New Arrivals - latest products
            var newArrivals = await _context.Sanphams
                .Where(s => s.Soluong > 0)
                .OrderByDescending(s => s.NgayDang)
                .Take(8)
                .ToListAsync();

            ViewBag.BestSellers = bestSellers;
            ViewBag.NewArrivals = newArrivals;

            return View();
        }

        public IActionResult ShoppingGuide()
        {
            return View();
        }

        public IActionResult ReturnPolicy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}
