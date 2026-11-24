# DoAnASP - E-commerce Clothing Website

Đồ án tốt nghiệp ASP.NET Core MVC - Website bán quần áo trực tuyến

## 🚀 Tổng quan

DoAnASP là một website thương mại điện tử bán quần áo được xây dựng bằng **ASP.NET Core MVC 10.0** với **Entity Framework Core 10** và **SQL Server LocalDB**. Dự án triển khai đầy đủ các chức năng của một hệ thống bán hàng trực tuyến hiện đại.

## 🛠️ Công nghệ sử dụng

| Công nghệ | Phiên bản | Mô tả |
|-----------|-----------|-------|
| **ASP.NET Core MVC** | 10.0 | Web framework |
| **Entity Framework Core** | 10.0 | ORM |
| **SQL Server** | LocalDB | Cơ sở dữ liệu |
| **BCrypt.Net-Next** | Latest | Mã hóa mật khẩu |
| **Bootstrap** | 5.3.2 | UI Framework |
| **Bootstrap Icons** | 1.11.0 | Icon library |
| **Chart.js** | Latest | Biểu đồ thống kê |
| **Cookie Authentication** | Built-in | Xác thực người dùng |

## ✨ Tính năng chính

### 🛍️ Khách hàng (Customer)
- **Trang chủ**: Banner, danh mục sản phẩm, sản phẩm mới
- **Danh sách sản phẩm**: Tìm kiếm, lọc theo danh mục, sắp xếp giá, phân trang
- **Chi tiết sản phẩm**: Gallery ảnh, chọn số lượng, thêm vào giỏ, mua ngay, sản phẩm liên quan
- **Giỏ hàng**: Thêm/sửa/xóa, cập nhật số lượng, tóm tắt đơn hàng
- **Đặt hàng**: Form thông tin giao hàng, thanh toán COD, xác nhận đơn hàng
- **Quản lý đơn hàng**: Xem lịch sử, chi tiết, hủy đơn (chờ xử lý)
- **Hồ sơ cá nhân**: Cập nhật thông tin, đổi mật khẩu
- **Đăng ký/Đăng nhập**: Validation email, xác nhận mật khẩu, rate limiting

### 👨‍💼 Quản trị (Admin)
- **Dashboard**: Thống kê đơn hàng, khách hàng, sản phẩm
- **Quản lý sản phẩm**: CRUD, upload ảnh, soft delete
- **Quản lý đơn hàng**: Xem, cập nhật trạng thái, khôi phục kho khi hủy
- **Quản lý khách hàng**: Xem danh sách, sửa thông tin, khóa/mở tài khoản, reset mật khẩu

## 📁 Cấu trúc dự án

```
DoAnASP/
├── Areas/
│   └── Admin/
│       ├── Controllers/       # Admin controllers
│       └── Views/             # Admin views (Dashboard, Sanphams, Donhangs, Khachhangs)
├── Controllers/               # Customer controllers (Home, Account, Cart, Orders, Sanphams)
├── Models/                    # Entity models, DbContext, CartItem
├── Views/
│   ├── Home/                  # Trang chủ, ShoppingGuide, ReturnPolicy, Contact
│   ├── Account/               # Login, Register, Profile
│   ├── Cart/                  # Giỏ hàng
│   ├── Orders/                # Đặt hàng, lịch sử đơn hàng
│   ├── Sanphams/              # Danh sách & chi tiết sản phẩm
│   └── Shared/                # Layouts, partials, error pages
├── Filters/                   # LayoutSelectorFilter
├── wwwroot/
│   ├── css/                   # Stylesheets
│   ├── js/                    # JavaScript (site.js, admin.js)
│   └── images/                # Uploaded product images
├── Data/                      # Migrations (nếu dùng)
├── qlquanao_sqlserver.sql     # Database script
└── Program.cs                 # Entry point & DI configuration
```

## 🗄️ Cơ sở dữ liệu

### Các bảng chính
- **Taikhoan**: Người dùng (khách hàng, admin)
- **Sanpham**: Sản phẩm
- **Donhang**: Đơn hàng
- **Chitietdonhang**: Chi tiết đơn hàng

### Các View báo cáo
- **v_chitiet_donhang**: Chi tiết đơn hàng cho báo cáo

### Tài khoản mặc định (password: 123456)
| Email | Role |
|-------|------|
| admin@doanasp.com | admin |
| user@doanasp.com | user |

## 🚀 Cài đặt và chạy

### Yêu cầu
- .NET 10 SDK
- SQL Server LocalDB (hoặc SQL Server Express/Full)

### Các bước

1. **Clone dự án**
```bash
git clone <repository-url>
cd DoAnASP
```

2. **Cấu hình Connection String** (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=qlquanao;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Tạo database từ script SQL**
```bash
# Mở SQL Server Management Studio hoặc Azure Data Studio
# Chạy file: qlquanao_sqlserver.sql
```

4. **Chạy ứng dụng**
```bash
dotnet run
```

5. **Truy cập**
- Trang khách: `https://localhost:xxxx/`
- Trang admin: `https://localhost:xxxx/Admin`
- Đăng nhập admin: `admin@doanasp.com` / `123456`

## 📝 Các quyết định thiết kế

### Soft Delete cho sản phẩm
- Sản phẩm có đơn hàng liên quan **không xóa vật lý**
- Thay vào đó: set `Soluong = 0`, thêm prefix `[Đã ngưng bán]` vào mô tả
- Giữ nguyên lịch sử đơn hàng và báo cáo

## 🔒 Bảo mật

- **Mật khẩu**: Hash bằng BCrypt (work factor mặc định)
- **Xác thực**: Cookie Authentication với Claims (Role-based)
- **CSRF**: Anti-forgery token trên tất cả form POST
- **Rate Limiting**: 5 lần đăng nhập/đăng ký mỗi phút per IP
- **Authorization**: `[Authorize(Roles = "admin")]` cho khu vực Admin

## 📊 Tiến độ dự án

Xem chi tiết tại [PROJECT_PROGRESS.md](PROJECT_PROGRESS.md)

## 🤝 Đóng góp

Đây là đồ án tốt nghiệp cá nhân. Mọi góp ý vui lòng tạo Issue.

## 📄 License

MIT License - Tự do sử dụng cho mục đích học tập, nghiên cứu.
