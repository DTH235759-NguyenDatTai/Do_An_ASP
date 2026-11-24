# Tiến độ dự án DoAnASP

**Cập nhật lần cuối:** 25/11/2026

**Công nghệ:** ASP.NET Core MVC, .NET 10, Entity Framework Core, SQL Server

## Tổng quan

Dự án là website bán quần áo gồm giao diện khách hàng và khu vực quản trị. Các chức năng chính đã được triển khai trong mã nguồn; một số luồng cần được xác nhận lại khi chạy với database đang sử dụng.

## Chức năng khách hàng

- Đăng ký, đăng nhập bằng cookie authentication; mật khẩu được băm bằng BCrypt.
- Hồ sơ cá nhân, đổi mật khẩu, lịch sử và chi tiết đơn hàng.
- Danh sách sản phẩm có lọc, sắp xếp và phân trang; trang chi tiết có sản phẩm liên quan.
- Giỏ hàng lưu trong session, cập nhật số lượng bằng AJAX; nút giỏ hàng trên thanh điều hướng mở thẳng trang giỏ hàng.
- Checkout kiểm tra giá và tồn kho phía server, tạo đơn hàng trong transaction.
- Khách có thể hủy đơn đang ở trạng thái `dang_xu_ly`; số lượng tồn được hoàn lại.

## Chức năng quản trị

- Dashboard hiển thị tổng sản phẩm, đơn hàng, khách hàng, đơn chờ xử lý, sản phẩm sắp hết hàng và đơn mới nhất.
- Quản lý sản phẩm: tạo, sửa, xem, xóa/ẩn; upload ảnh giới hạn định dạng và dung lượng. Đường dẫn ảnh được chuẩn hóa cho cả tên file và URL.
- Quản lý đơn hàng và khách hàng.
- Không còn trang thống kê doanh thu hoặc phần sản phẩm bán chạy trong giao diện admin.
- Cập nhật trạng thái đơn hàng được giới hạn theo luồng hiện tại: `dang_xu_ly` có thể chuyển sang `da_giao` hoặc `huy`; đơn đã giao hoặc đã hủy không có bước chuyển tiếp.

## Database và migration

- Cấu hình SQL Server lấy từ `appsettings.json`; `QlquanaoContext` không chứa connection string hardcode.
- Model `Sanpham` không còn các trường `ProductType`, `TryonImage`, `TryonType`, tương ứng các cột đã xóa khỏi database.
- Migration `InitialCreate` hiện là baseline rỗng để ghi nhận schema có sẵn mà không tạo hoặc xóa bảng. Trước khi chạy `Update-Database`, cần xác nhận connection string trỏ đúng database hiện có và dữ liệu cần giữ.
- Script SQL khởi tạo nằm ở `qlquanao_asp.sql`.

## Giao diện và hạ tầng

- Layout riêng cho khách hàng và admin; có trang hướng dẫn mua hàng, đổi trả, liên hệ, lỗi chung và 404.
- Có antiforgery cho form/AJAX, phân quyền theo role và rate limiting cho đăng nhập/đăng ký.
- Cookie đăng nhập không duy trì sau khi ứng dụng khởi động lại; người dùng cần đăng nhập lại.
- Trang chi tiết sản phẩm hiển thị số lượng đã bán và sản phẩm liên quan.

## Việc cần xác nhận

- Chạy `Update-Database -Context QlquanaoContext` trên đúng database và xác nhận migration baseline được ghi vào `__EFMigrationsHistory`.
- Kiểm tra trực tiếp luồng cập nhật trạng thái đơn hàng trên ứng dụng đang chạy. Mã nguồn hiện gửi riêng trạng thái, nạp đơn hàng từ database, kiểm tra chuyển trạng thái rồi lưu; cần khởi động lại ứng dụng sau khi build để dùng mã mới.
- Xác nhận ảnh sản phẩm hiện có trong database trỏ tới file thực tế trong `wwwroot/images` hoặc URL còn hoạt động.

## Kiểm tra gần nhất

- `dotnet build DoAnASP/DoAnASP.csproj --no-restore` đã build thành công ở các lần chỉnh sửa gần đây.
- Chưa xác nhận trực tiếp thao tác với database hiện tại sau khi tạo baseline.
