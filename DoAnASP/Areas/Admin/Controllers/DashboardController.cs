using DoAnASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
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

            ViewBag.TongSanPham = tongSanPham;
            ViewBag.TongDonHang = tongDonHang;
            ViewBag.TongKhachHang = tongKhachHang;
            ViewBag.DonHangMoi = donHangMoi;
            ViewBag.SanPhamSapHet = sanPhamSapHet;
            ViewBag.DonHangMoiNhat = donHangMoiNhat;
            return View();
        }
    }
}
