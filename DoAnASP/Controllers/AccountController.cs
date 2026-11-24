using System.Security.Claims;
using DoAnASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [EnableRateLimiting("LoginLimiter")]
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
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false, AllowRefresh = false });

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
        [EnableRateLimiting("LoginLimiter")]
        public async Task<IActionResult> Register(string name, string email, string password, string confirmPassword, string? sdt, string? diaChi)
        {
            name = (name ?? "").Trim();
            email = (email ?? "").Trim();

            // Validate required fields
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đủ họ tên, email và mật khẩu.";
                return View();
            }

            // Validate email format
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    ViewBag.Error = "Định dạng email không hợp lệ.";
                    return View();
                }
            }
            catch
            {
                ViewBag.Error = "Định dạng email không hợp lệ.";
                return View();
            }

            // Validate password length
            if (password.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự.";
                return View();
            }

            // Validate password confirmation
            if (password != confirmPassword)
            {
                ViewBag.Error = "Xác nhận mật khẩu không khớp.";
                return View();
            }

            // Check duplicate email
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

        // ---------- HỒ SƠ CÁ NHÂN ----------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _db.Taikhoans.FindAsync(userId);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(string name, string sdt, string diaChi)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _db.Taikhoans.FindAsync(userId);
            if (user == null)
                return NotFound();

            name = (name ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("Name", "Họ tên không được để trống.");
            }

            if (ModelState.IsValid)
            {
                user.Name = name;
                user.Sdt = sdt;
                user.DiaChi = diaChi;
                await _db.SaveChangesAsync();

                // Cập nhật lại claim Name trong cookie
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Name),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role.Trim())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = false, AllowRefresh = false });

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Profile));
            }

            return View(user);
        }

        // ---------- ĐỔI MẬT KHẨU ----------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _db.Taikhoans.FindAsync(userId);
            if (user == null)
                return NotFound();

            if (!CheckPassword(currentPassword, user.Password))
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction(nameof(Profile));
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction(nameof(Profile));
            }

            if (newPassword != confirmNewPassword)
            {
                TempData["ErrorMessage"] = "Xác nhận mật khẩu không khớp.";
                return RedirectToAction(nameof(Profile));
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Profile));
        }

        // ---------- ĐĂNG XUẤT ----------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static bool CheckPassword(string? password, string hash)
        {
            try { return BCrypt.Net.BCrypt.Verify(password ?? "", hash.Trim()); }
            catch { return false; } // hash trong DB sai định dạng -> coi như sai mật khẩu
        }
    }
}
