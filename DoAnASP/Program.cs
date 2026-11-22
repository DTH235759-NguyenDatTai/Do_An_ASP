using DoAnASP.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<QlquanaoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";            // chưa đăng nhập -> về đây
        options.AccessDeniedPath = "/Account/AccessDenied"; // đăng nhập nhưng sai quyền -> về đây
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Add session support (cần cho giỏ hàng CartController)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();          // phải đứng SAU UseRouting, TRƯỚC UseAuthentication
app.UseAuthentication();   // phải đứng TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

// Route cho khu vực Admin (file cũ của bạn thiếu route này nên /Admin/... bị 404)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();