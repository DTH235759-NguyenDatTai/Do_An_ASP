using DoAnASP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly QlquanaoContext _context;

        public DashboardController(QlquanaoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Thống kê tổng quan
            var tongSanPham = await _context.Sanphams.CountAsync();
            var tongDonHang = await _context.Donhangs.CountAsync();
            var tongKhachHang = await _context.Taikhoans.CountAsync();

            var doanhThu = await _context.Donhangs
                .Where(d => d.TrangThai == "da_giao")
                .SumAsync(d => (decimal?)d.TongTien) ?? 0;

            var donHangMoi = await _context.Donhangs
                .Where(d => d.TrangThai == "dang_xu_ly")
                .CountAsync();

            var sanPhamSapHet = await _context.Sanphams
                .Where(p => p.Soluong <= 5)
                .CountAsync();

            // 5 đơn hàng mới nhất
            var donHangMoiNhat = await _context.Donhangs
                .Include(d => d.IdNguoidatNavigation)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .ToListAsync();

            // 5 sản phẩm bán chạy nhất
            var spBanChay = await _context.VSpBanchays
                .OrderByDescending(v => v.DaBan)
                .Take(5)
                .ToListAsync();

            ViewBag.TongSanPham = tongSanPham;
            ViewBag.TongDonHang = tongDonHang;
            ViewBag.TongKhachHang = tongKhachHang;
            ViewBag.DoanhThu = doanhThu;
            ViewBag.DonHangMoi = donHangMoi;
            ViewBag.SanPhamSapHet = sanPhamSapHet;
            ViewBag.DonHangMoiNhat = donHangMoiNhat;
            ViewBag.SpBanChay = spBanChay;

            return View();
        }
    }
}