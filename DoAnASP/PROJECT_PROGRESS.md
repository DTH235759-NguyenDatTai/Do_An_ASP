# PROJECT PROGRESS - DoAnASP

## Frontend typography update (2026-11-24)
- Unified customer and admin font stacks using system UI fonts with Segoe UI and Arial fallbacks for Vietnamese glyph support, without relying on a remote font download.
- Confirmed both layouts declare UTF-8; the garbled Vietnamese seen in PowerShell output came from console rendering, not corrupted view text.
- Verified the project builds with 0 errors and 0 warnings after the update.

**Ngày cập nhật**: 2026-11-24
**Trạng thái**: Hoàn thành các phase chính, sẵn sàng triển khai

---

## 📋 TỔNG QUAN TIẾN ĐỘ

| Phase | Trạng thái | Hoàn thành |
|-------|------------|------------|
| **Phase 1: Critical Fixes** | ✅ Hoàn thành | 100% |
| **Phase 2: Order Flow** | ✅ Hoàn thành | 100% |
| **Phase 3: Product Detail & Home Page** | ✅ Hoàn thành | 100% |
| **Phase 4: Polish & Extras** | ✅ Hoàn thành | 100% |

---

## ✅ PHASE 1: CRITICAL FIXES (Hoàn thành)

### 1.1 CSRF Token cho AJAX Cart
- [x] Thêm form ẩn `#antiforgery-form` trong `_LayoutCustomer.cshtml`
- [x] Cập nhật `site.js` để lấy token từ form ẩn
- [x] Cập nhật `Cart/Index.cshtml` và `_CartPartial.cshtml`

### 1.2 Xóa Controller/View trùng lặp
- [x] Xóa `Controllers/DonhangsController.cs`
- [x] Xóa `Controllers/ChitietdonhangsController.cs`
- [x] Xóa `Controllers/TaikhoansController.cs`
- [x] Xóa thư mục `Views/Donhang/`, `Views/Chitietdonhang/`, `Views/Taikhoan/`
- [x] Xóa `Views/Sanphams/Create.cshtml`, `Edit.cshtml`, `Delete.cshtml` (gốc - chỉ dùng Area Admin)


### 1.4 Layout & Navigation
- [x] Đăng ký `LayoutSelectorFilter` global trong `Program.cs`
- [x] Sửa `_LayoutAdmin.cshtml`: link `Taikhoans` → `Khachhangs`, `huy` → `da_huy`
- [x] Sửa `KhachhangsController.Edit`: không bind Password, check email unique, ngăn self-demote, thêm ResetPassword
- [x] Thêm `_LoginPartial.cshtml`: link "Hồ sơ của tôi", "Đơn hàng của tôi"

### 1.5 AccountController
- [x] Thêm `Profile` (GET/POST)
- [x] Thêm `ChangePassword` (POST)

### 1.6 CartItem Model
- [x] Di chuyển từ Controller sang `Models/CartItem.cs`
- [x] Cập nhật `CartController`, `Index.cshtml`, `_CartPartial.cshtml`

### 1.7 SanphamsController (Customer)
- [x] Thêm tham số `sort` (price_asc, price_desc)
- [x] Cập nhật `Views/Sanphams/Index.cshtml`: nút sắp xếp, phân trang giữ sort

### 1.8 Xóa Identity & Package không dùng
- [x] Xóa thư mục `Areas/Identity/`, `Data/ApplicationDbContext.cs`, `Data/Migrations/`
- [x] Xóa `Views/Shared/_Layout.cshtml` và `_Layout.cshtml.css`
- [x] Gỡ `Microsoft.AspNetCore.Identity.*` khỏi `.csproj`
- [x] Xóa `OnConfiguring` hardcoded connection string trong `QlquanaoContext.cs`

### 1.9 Database
- [x] Chạy `qlquanao_sqlserver.sql` thành công
- [x] Tạo tables, views, sample data, admin accounts (BCrypt hash)

---

## ✅ PHASE 2: ORDER FLOW (Hoàn thành)

### 2.1 OrdersController (Mới)
- [x] Tạo `Controllers/OrdersController.cs`
- [x] `Checkout` (GET/POST) - validate giá/kho server-side, transaction
- [x] `Success` - trang xác nhận đơn hàng
- [x] `MyOrders` - lịch sử đơn hàng có phân trang
- [x] `Details` - chi tiết đơn hàng
- [x] `Cancel` - hủy đơn (chỉ "dang_xu_ly"), khôi phục kho

### 2.2 Views/Orders
- [x] `Checkout.cshtml` - form giao hàng + tóm tắt đơn hàng sidebar
- [x] `Success.cshtml` - trang thành công
- [x] `MyOrders.cshtml` - danh sách có phân trang, nút hủy
- [x] `Details.cshtml` - chi tiết đầy đủ

### 2.3 Admin DonhangsController
- [x] Validate chuyển trạng thái: `dang_xu_ly` → `da_giao` / `da_huy`
- [x] Khôi phục kho khi hủy đơn (`da_huy`)

### 2.4 Navigation
- [x] Cập nhật link "Thanh toán" → `/Orders/Checkout` trong Cart views
- [x] Thêm "Đơn hàng của tôi" vào `_LoginPartial.cshtml`

---

## ✅ PHASE 3: PRODUCT DETAIL & HOME PAGE (Hoàn thành)

### 3.1 Sanphams/Details.cshtml (Redesign hoàn toàn)
- [x] Gallery ảnh (carousel + thumbnail)
- [x] Badge danh mục, giới tính
- [x] Chọn số lượng + nút "Thêm vào giỏ" (AJAX)
- [x] Nút "Mua ngay" → redirect Checkout
- [x] Fetch sold count qua API `/Sanphams/GetSoldCount/{id}`
- [x] Sản phẩm liên quan (cùng loại, còn hàng, max 4)
- [x] Meta: đã bán, ngày đăng

### 3.2 SanphamsController
- [x] Thêm `GetSoldCount(int id)` endpoint
- [x] Load `RelatedProducts` trong `Details`

### 3.3 Home/Index.cshtml (Redesign hoàn toàn)
- [x] Hero section gradient + CTA
- [x] Danh mục 5 cột: Áo, Quần, Váy, Đồ bộ, Giày dép
- [x] Sản phẩm mới (theo `NgayDang` desc)
- [x] Features: ship free, đổi trả, COD, support 24/7

### 3.4 HomeController
- [x] Inject `QlquanaoContext`
- [x] Load `NewArrivals` từ `Sanphams` order by `NgayDang` desc

---

## ✅ PHASE 4: POLISH & EXTRAS (Hoàn thành)

### 4.1 Admin Product Image Upload
- [x] Thêm `IFormFile` parameter (`HinhAnhFile`) trong Admin `SanphamsController`
- [x] `SaveImageAsync`: validate extension, size (5MB), unique filename (Guid)
- [x] `DeleteImage`: xóa file cũ khi cập nhật/xóa
- [x] Soft delete: check `Chitietdonhangs` trước khi xóa
- [x] Cập nhật `Create.cshtml`, `Edit.cshtml`: input type=file + fallback URL

### 4.2 Footer Links (Thay # bằng trang thật)
- [x] Thêm `HomeController`: `ShoppingGuide()`, `ReturnPolicy()`, `Contact()`
- [x] Tạo Views: `ShoppingGuide.cshtml`, `ReturnPolicy.cshtml`, `Contact.cshtml`
- [x] Cập nhật `_LayoutCustomer.cshtml` footer
- [x] Cập nhật `_LayoutAdmin.cshtml` footer

### 4.3 Registration Improvements
- [x] Validate email format (`System.Net.Mail.MailAddress`)
- [x] Thêm trường `confirmPassword`
- [x] Validation: password ≥ 6 ký tự, khớp confirm
- [x] Cập nhật `Views/Account/Register.cshtml`

### 4.4 Rate Limiting
- [x] Thêm `AddRateLimiter` trong `Program.cs`
- [x] `LoginLimiter`: 5 requests/phút/IP
- [x] `GlobalLimiter`: 100 requests/phút/IP
- [x] Áp dụng `[EnableRateLimiting("LoginLimiter")]` cho `Login` và `Register` POST

### 4.5 Error Pages
- [x] `Views/Shared/Error.cshtml` - trang lỗi chung
- [x] `Views/Shared/NotFound.cshtml` - trang 404
- [x] `HomeController.NotFound()` action
- [x] `UseStatusCodePagesWithReExecute("/Home/NotFound")` trong `Program.cs`

### 4.6 Documentation
- [x] `README.md` - tài liệu dự án đầy đủ
- [x] `PROJECT_PROGRESS.md` - file này

---

## ⚠️ VẤN ĐỀ ĐÃ GIẢI QUYẾT (Build Errors)

| Lỗi | Nguyên nhân | Giải pháp |
|-----|-------------|-----------|
| `CS0246: AuthorizeAttribute not found` | Gỡ Identity package | `Authorize` thuộc `Microsoft.AspNetCore.Mvc.Core` (transitive) - OK |
| `CS1501: ToString overload` | Nullable DateOnly/decimal | Dùng `?.ToString("format")` hoặc kiểm tra null |
| `CS8625: Nullable warning _LayoutAdmin` | `Active()` param null | Dùng `""` thay vì `null` |
| `BLAZOR102: Scoped CSS` | File `_Layout.cshtml.css` orphan | Xóa file |
| `CS4032: await in non-async` | `UpdateQuantity` | Làm method `async` |
| `CS1061: GetAndStoreTokens` | Sai method name | Dùng `@Html.AntiForgeryToken()` |
| `RZ1006: Missing }` | Razor syntax | Sửa `@section Scripts` đóng đúng |

---

## 🗂️ FILES THAY ĐỔI CHÍNH

### Controllers (Mới/Sửa)
- `Controllers/OrdersController.cs` (MỚI)
- `Controllers/HomeController.cs` (Sửa: DI context, load data, error pages)
- `Controllers/AccountController.cs` (Sửa: Register validation, RateLimit)
- `Controllers/SanphamsController.cs` (Sửa: sort, GetSoldCount, RelatedProducts)
- `Areas/Admin/Controllers/SanphamsController.cs` (Sửa: IFormFile upload, soft delete)
- `Areas/Admin/Controllers/KhachhangsController.cs` (Sửa: Edit validation)
- `Areas/Admin/Controllers/DonhangsController.cs` (Sửa: status validation, stock restore)

### Views (Mới/Sửa)
- `Views/Home/Index.cshtml` (MỚI - redesign hoàn toàn)
- `Views/Home/ShoppingGuide.cshtml` (MỚI)
- `Views/Home/ReturnPolicy.cshtml` (MỚI)
- `Views/Home/Contact.cshtml` (MỚI)
- `Views/Sanphams/Details.cshtml` (MỚI - redesign hoàn toàn)
- `Views/Orders/*.cshtml` (4 file MỚI)
- `Views/Account/Register.cshtml` (Sửa: confirm password)
- `Views/Shared/_LayoutCustomer.cshtml` (Sửa: antiforgery form, footer links)
- `Views/Shared/_LayoutAdmin.cshtml` (Sửa: sidebar links, footer links)
- `Views/Shared/_LoginPartial.cshtml` (Sửa: profile, orders links)
- `Views/Shared/Error.cshtml` (MỚI)
- `Views/Shared/NotFound.cshtml` (MỚI)
- `Areas/Admin/Views/Sanphams/Create.cshtml` (Sửa: file upload)
- `Areas/Admin/Views/Sanphams/Edit.cshtml` (Sửa: file upload)
- `Views/Cart/Index.cshtml` (Sửa: checkout link, token)
- `Views/Cart/_CartPartial.cshtml` (Sửa: checkout link, model)

### Models
- `Models/CartItem.cs` (MỚI - di chuyển từ Controller)

### Config
- `Program.cs` (Sửa: RateLimiter, StatusCodePages, LayoutSelectorFilter)
- `DoAnASP.csproj` (Sửa: gỡ Identity packages)
- `Models/QlquanaoContext.cs` (Sửa: xóa OnConfiguring)
- `.gitignore` (MỚI)

### Database
- `qlquanao_sqlserver.sql` - Script tạo DB hoàn chỉnh

---

## 🎯 CHECKLIST TRIỂN KHAI (Production Ready)

- [x] Connection string từ `appsettings.json` (không hardcode)
- [x] BCrypt hash cho mật khẩu (không lưu plain text)
- [x] Anti-forgery token trên tất cả form POST
- [x] Role-based authorization (admin/user)
- [x] Rate limiting đăng nhập/đăng ký
- [x] Soft delete sản phẩm có đơn hàng
- [x] Upload ảnh an toàn (validate ext, size, unique name)
- [x] Transaction khi đặt hàng (rollback nếu lỗi)
- [x] Khôi phục kho khi hủy đơn (customer + admin)
- [x] Custom error pages (404, 500)
- [x] Responsive UI (Bootstrap 5)
- [x] AJAX cart với toast notification
- [x] Pagination cho danh sách sản phẩm, đơn hàng, khách hàng
- [x] Chart.js cho thống kê admin

---

## 📝 GHI CHÚ KHÁC

### Tài khoản test (password: 123456)
| Email | Role |
|-------|------|
| admin@doanasp.com | admin |
| user@doanasp.com | user |

### Cách test nhanh
```bash
dotnet run
# Mở https://localhost:xxxx/
# Đăng nhập admin: /Admin
```

### Các thư mục upload
- `wwwroot/images/` - Tự tạo khi chạy, chứa ảnh sản phẩm upload

---

**Kết luận**: Dự án đã hoàn thành tất cả yêu cầu trong audit report. Sẵn sàng cho bảo vệ đồ án tốt nghiệp.
