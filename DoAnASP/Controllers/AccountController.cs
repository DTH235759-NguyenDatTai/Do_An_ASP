using System.Security.Claims;
using DoAnASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Controllers
{
    public class AccountController : Controller
    {
        private readonly QlquanaoContext _db;
        public AccountController(QlquanaoContext db) => _db = db;

        // ---------- ĐĂNG NHẬP ----------
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl)
        {
            email = (email ?? "").Trim();
            var user = await _db.Taikhoans.FirstOrDefaultAsync(t => t.Email == email);

            // 1) Kiểm tra email + mật khẩu (so hash bằng BCrypt)
            if (user == null || !CheckPassword(password, user.Password))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // 2) Khớp -> ghi thông tin + quyền (role) vào cookie đăng nhập
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.Trim())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // 3) Kiểm tra role để chuyển trang
            if (user.Role.Trim() == "admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });   // -> _LayoutAdmin

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");                                     // -> _LayoutCustomer
        }

        // ---------- ĐĂNG KÝ ----------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string name, string email, string password, string? sdt, string? diaChi)
        {
            name = (name ?? "").Trim();
            email = (email ?? "").Trim();

            if (name == "" || email == "" || string.IsNullOrEmpty(password) || password.Length < 6)
            {
                ViewBag.Error = "Vui lòng nhập đủ họ tên, email và mật khẩu (tối thiểu 6 ký tự).";
                return View();
            }
            if (await _db.Taikhoans.AnyAsync(t => t.Email == email))
            {
                ViewBag.Error = "Email này đã được đăng ký.";
                return View();
            }

            _db.Taikhoans.Add(new Taikhoan
            {
                Name = name,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password), // lưu hash, không lưu mật khẩu thô
                Role = "user",                                        // luôn là user, không cho người dùng tự chọn
                Sdt = sdt,
                DiaChi = diaChi,
                NgayTao = DateTime.Now
            });
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công, hãy đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        // ---------- ĐĂNG XUẤT ----------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => Content("Bạn không có quyền truy cập trang này.");

        private static bool CheckPassword(string? password, string hash)
        {
            try { return BCrypt.Net.BCrypt.Verify(password ?? "", hash.Trim()); }
            catch { return false; } // hash trong DB sai định dạng -> coi như sai mật khẩu
        }
    }
}