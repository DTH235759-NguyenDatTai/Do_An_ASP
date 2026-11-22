# DoAnASP - Project Progress Documentation

## Project Overview
ASP.NET Core MVC e-commerce application for clothing store (DoAnASP)

## Current Status: ✅ Login/Registration System Complete

---

## Completed Features

### 1. Product Listing (Sanpham/Index)
- **File**: `Views/Sanphams/Index.cshtml`
- **Controller**: `Controllers/SanphamsController.cs`
- Features:
  - Responsive product grid (1/2/3/4 columns)
  - Search by product name/description
  - Filter by category (LoaiSp)
  - Pagination (12 items/page)
  - Sort options (newest, price low→high, price high→low)
  - Product cards with image, price, stock status
  - "Add to Cart" button with stock validation
  - Out-of-stock badge
  - Helper function `GetImageUrl()` for wwwroot/images

### 2. Shopping Cart System
- **Controller**: `Controllers/CartController.cs`
- **Views**: `Views/Cart/Index.cshtml`, `Views/Cart/_CartPartial.cshtml`
- Features:
  - Session-based cart persistence
  - Add/Update/Remove items
  - Quantity controls
  - Cart total calculation
  - Navbar dropdown cart preview
  - AJAX operations with toast notifications

### 3. Authentication System (NEW)
- **Controller**: `Controllers/AccountController.cs`
- **Views**: 
  - `Views/Account/Login.cshtml` - AJAX login with toast notifications
  - `Views/Account/Register.cshtml` - Registration form
  - `Views/Account/Profile.cshtml` - User profile with change password
  - `Views/Account/AccessDenied.cshtml` - Access denied page
- **Middleware**: Cookie Authentication (`Program.cs`)
- **Filter**: `Filters/LayoutSelectorFilter.cs` - Role-based layout selection

#### Login Features:
- AJAX form submission
- Toast notifications (success/error)
- Remember me (30 days persistent cookie)
- Role-based redirect:
  - Admin → `/Admin/Dashboard` (Admin layout)
  - User → `/Home/Index` (Customer layout)
- Debug logging for password verification

#### Registration Features:
- Full validation (name, email, password, confirm password)
- Auto-login after registration
- Default role: "user"
- Toast success notification

#### Profile Features:
- Edit name, phone, address
- Change password modal
- Claim updates on profile change

### 4. Layout System
- **Customer Layout**: `Views/Shared/_LayoutCustomer.cshtml`
  - Navbar with cart dropdown
  - Category links in footer
  - Dynamic cart count badge
  - Loads cart dropdown on demand via AJAX

- **Admin Layout**: `Views/Shared/_LayoutAdmin.cshtml` (existing)
  - Sidebar navigation
  - Admin dashboard, products, orders, statistics, customers

- **Dynamic Layout Selection**: `_ViewStart.cshtml` + `LayoutSelectorFilter`
  - Automatically selects layout based on user role claim
  - Admin users see Admin layout
  - Regular users see Customer layout

---

## Database Schema (QlquanaoContext)

### Tables:
- `taikhoan` - Users (id, name, email, password, role, sdt, dia_chi, ngay_tao)
- `sanpham` - Products
- `donhang` - Orders
- `chitietdonhang` - Order details

### Existing Admin Users:
| Email | Role | Name |
|-------|------|------|
| admin@gmail.com | admin | admin |
| tai27@gmail.com | admin | Tai |

> **Note**: Passwords are hashed with SHA256 + "DoAnASP_Salt". Original passwords unknown.

---

## Key Technical Details

### Password Hashing
```csharp
// HashPassword() uses SHA256 with salt
private string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "DoAnASP_Salt"));
    return Convert.ToBase64String(hashedBytes);
}
```

### Cookie Authentication Config (Program.cs)
```csharp
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
        options.Cookie.Name = "DoAnASP.Auth";
    });
```

### Role-Based Layout Filter
```csharp
// Filters/LayoutSelectorFilter.cs
// Runs on every action, sets ViewData["Layout"] based on role claim
if (user.IsInRole("admin")) layout = "_LayoutAdmin.cshtml";
else layout = "_LayoutCustomer.cshtml";
```

### Image Path Helper (Views)
```csharp
Func<string, string> GetImageUrl = (hinhAnh) => {
    if (string.IsNullOrEmpty(hinhAnh)) return placeholder;
    if (hinhAnh.StartsWith("http") || hinhAnh.StartsWith("/")) return hinhAnh;
    return "/images/" + hinhAnh; // wwwroot/images/
};
```

---

## How to Test

### 1. Start Application
```bash
cd D:\Code\ASP\DoAnASP\DoAnASP
dotnet run
# Runs on http://localhost:5162 (or check launchSettings.json)
```

### 2. Test Customer Flow
1. Navigate to `/Sanphams` - Product listing
2. Click "Đăng ký" - Register new user (defaults to "user" role)
3. Login → Redirects to `/Home/Index` with Customer layout
4. Add products to cart → Navbar cart dropdown works

### 3. Test Admin Flow
1. **Option A**: Update user role in database:
   ```sql
   UPDATE taikhoan SET role = 'admin' WHERE email = 'your@email.com';
   ```
2. **Option B**: Use existing admin accounts (need password)
3. Login as admin → Redirects to `/Admin/Dashboard` with Admin layout

### 4. Test AJAX Login
- Open browser DevTools → Network tab
- Submit login form
- Should return JSON: `{success: true, message: "...", redirectUrl: "...", role: "admin|user"}`
- Toast notification appears
- Auto-redirect after 800ms

---

## Known Issues / TODO

### High Priority
- [ ] **Password for existing admin accounts unknown** - Cannot test original admin logins
- [ ] Forgot password functionality not implemented
- [ ] Email verification not implemented

### Medium Priority
- [ ] Add unit tests for AccountController
- [ ] Implement proper password hashing (BCrypt/Argon2) instead of SHA256
- [ ] Add rate limiting on login endpoint
- [ ] Admin user management (CRUD for users in Admin area)

### Low Priority
- [ ] Social login (Google, Facebook) - buttons exist but disabled
- [ ] Product image upload in Admin area
- [ ] Order management for customers
- [ ] Payment integration

---

## File Structure Summary

```
Controllers/
├── AccountController.cs      # NEW - Auth system
├── CartController.cs         # NEW - Shopping cart
├── SanphamsController.cs     # Updated - Product listing
├── HomeController.cs
└── Areas/Admin/Controllers/  # Admin area (existing)

Views/
├── Account/                  # NEW - Auth views
│   ├── Login.cshtml
│   ├── Register.cshtml
│   ├── Profile.cshtml
│   └── AccessDenied.cshtml
├── Cart/                     # NEW - Cart views
│   ├── Index.cshtml
│   └── _CartPartial.cshtml
├── Sanphams/
│   └── Index.cshtml          # Updated - Product grid
├── Shared/
│   ├── _LayoutCustomer.cshtml # Updated - Navbar cart
│   ├── _LayoutAdmin.cshtml    # Existing
│   ├── _LoginPartial.cshtml   # Updated - Uses Account controller
│   └── _ViewStart.cshtml      # Updated - Dynamic layout
└── _ViewImports.cshtml

Filters/
└── LayoutSelectorFilter.cs   # NEW - Role-based layout

wwwroot/
├── js/site.js                # Updated - Toast, cart functions
├── css/site.css
└── images/                   # Product images

Models/
├── Taikhoan.cs               # User model with Role
├── Sanpham.cs                # Product model
└── QlquanaoContext.cs        # DbContext

Program.cs                    # Updated - Cookie auth, session, filter registration
```

---

## For Next AI Assistant

### To Continue Work:
1. **Read this file first** to understand project state
2. **Run `dotnet build`** to verify compilation
3. **Run `dotnet run`** to start application
4. **Check database** for existing users: `sqlcmd -S "(localdb)\mssqllocaldb" -d "qlquanao" -Q "SELECT email, role FROM taikhoan"`

### To Make Existing Admin Work:
```sql
-- Update password for admin@gmail.com to known value (e.g., "admin123")
-- Hash: SHA256("admin123" + "DoAnASP_Salt") = compute and update
UPDATE taikhoan SET password = '<computed_hash>' WHERE email = 'admin@gmail.com';
```

### Quick Password Hash Generator (C#):
```csharp
using var sha256 = SHA256.Create();
var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes("yourpassword" + "DoAnASP_Salt")));
Console.WriteLine(hash);
```

---

## Last Updated
**Date**: 2026-11-20  
**Session**: Implemented complete authentication system with role-based layouts, AJAX login with toast notifications, and documentation

---

## Git Status
```bash
git status
# Modified: Program.cs, Controllers/*, Views/*, Filters/*, wwwroot/js/site.js
# New: Controllers/AccountController.cs, Controllers/CartController.cs, Filters/LayoutSelectorFilter.cs
# New Views: Account/*, Cart/*
```