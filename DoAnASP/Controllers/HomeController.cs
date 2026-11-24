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
            // New Arrivals - latest products
            var newArrivals = await _context.Sanphams
                .Where(s => s.Soluong > 0)
                .OrderByDescending(s => s.NgayDang)
                .Take(8)
                .ToListAsync();

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
