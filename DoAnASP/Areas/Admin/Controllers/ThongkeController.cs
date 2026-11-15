using DoAnASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongkeController : Controller
    {
        private readonly QlquanaoContext _context;

        public ThongkeController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Admin/Thongke
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            // Mặc định 30 ngày gần nhất
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddDays(-30);
            if (!toDate.HasValue) toDate = DateTime.Now;

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            // Convert to DateOnly for comparison with VDoanhthuNgay.Ngay
            var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
            var toDateOnly = DateOnly.FromDateTime(toDate.Value);

            // Doanh thu theo ngày
            var doanhThuNgay = await _context.VDoanhthuNgays
                .Where(v => v.Ngay >= fromDateOnly && v.Ngay <= toDateOnly)
                .OrderBy(v => v.Ngay)
                .ToListAsync();

            // Tổng doanh thu
            var tongDoanhThu = await _context.Donhangs
                .Where(d => d.TrangThai == "da_giao" && d.NgayDat >= fromDate && d.NgayDat <= toDate)
                .SumAsync(d => (decimal?)d.TongTien) ?? 0;

            // Tổng đơn hàng
            var tongDonHang = await _context.Donhangs
                .Where(d => d.NgayDat >= fromDate && d.NgayDat <= toDate)
                .CountAsync();

            // Đơn hàng đã giao
            var donHangDaGiao = await _context.Donhangs
                .Where(d => d.TrangThai == "da_giao" && d.NgayDat >= fromDate && d.NgayDat <= toDate)
                .CountAsync();

            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.TongDonHang = tongDonHang;
            ViewBag.DonHangDaGiao = donHangDaGiao;
            ViewBag.TyLeHoanThanh = tongDonHang > 0 ? (double)donHangDaGiao / tongDonHang * 100 : 0;

            return View(doanhThuNgay);
        }

        // GET: Admin/Thongke/SanphamBanChay
        public async Task<IActionResult> SanphamBanChay(int top = 10)
        {
            var spBanChay = await _context.VSpBanchays
                .OrderByDescending(v => v.DaBan)
                .Take(top)
                .ToListAsync();

            ViewBag.Top = top;
            return View(spBanChay);
        }
    }
}