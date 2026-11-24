/* =====================================================================
   CSDL: qlquanao  (Web bán quần áo - đồ án ASP)
   Phiên bản SQL Server (T-SQL) - chạy trong SSMS, SQL Server 2016 SP1 trở lên.
   Giữ nguyên tên bảng/cột cũ để không phải sửa code đang có.
   ===================================================================== */

USE master;
GO

IF DB_ID(N'qlquanao') IS NOT NULL
BEGIN
    ALTER DATABASE qlquanao SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE qlquanao;
END
GO

CREATE DATABASE qlquanao COLLATE Vietnamese_CI_AS;
GO

USE qlquanao;
GO

/* ---------------------------------------------------------------------
   Bảng tài khoản
   --------------------------------------------------------------------- */
CREATE TABLE taikhoan (
    id        INT IDENTITY(1,1) NOT NULL,
    name      NVARCHAR(100)  NOT NULL,
    email     NVARCHAR(100)  NOT NULL,
    [password] VARCHAR(255)  NOT NULL,           -- lưu hash bcrypt, KHÔNG lưu mật khẩu thô
    role      NVARCHAR(10)   NOT NULL CONSTRAINT DF_taikhoan_role DEFAULT N'user',
    sdt       VARCHAR(15)    NULL,
    dia_chi   NVARCHAR(255)  NULL,
    ngay_tao  DATETIME2(0)   NOT NULL CONSTRAINT DF_taikhoan_ngaytao DEFAULT SYSDATETIME(),
    CONSTRAINT PK_taikhoan PRIMARY KEY (id),
    CONSTRAINT UQ_taikhoan_email UNIQUE (email),
    CONSTRAINT CK_taikhoan_role CHECK (role IN (N'user', N'admin'))
);
GO

/* ---------------------------------------------------------------------
   Bảng sản phẩm
   --------------------------------------------------------------------- */
CREATE TABLE sanpham (
    id           INT IDENTITY(1,1) NOT NULL,
    ten_sp       NVARCHAR(150) NOT NULL,
    loai_sp      NVARCHAR(50)  NOT NULL,         -- Áo, Quần, Váy, Đồ bộ, Giày dép
    gt_sp        NVARCHAR(20)  NOT NULL CONSTRAINT DF_sanpham_gt DEFAULT N'Unisex',  -- Nam, Nữ, Unisex
    soluong      INT           NOT NULL CONSTRAINT DF_sanpham_soluong DEFAULT 0,
    gia          DECIMAL(12,0) NOT NULL CONSTRAINT DF_sanpham_gia DEFAULT 0,         -- giá VND
    hinh_anh     VARCHAR(255)  NULL,
    mo_ta        NVARCHAR(MAX) NULL,
    ngay_dang    DATETIME2(0)  NOT NULL CONSTRAINT DF_sanpham_ngaydang DEFAULT SYSDATETIME(),
    tryon_image  VARCHAR(255)  NULL,
    product_type VARCHAR(10)   NULL,
    tryon_type   VARCHAR(50)   NULL,
    CONSTRAINT PK_sanpham PRIMARY KEY (id),
    CONSTRAINT CK_sanpham_soluong CHECK (soluong >= 0),
    CONSTRAINT CK_sanpham_gia CHECK (gia >= 0),
    CONSTRAINT CK_sanpham_producttype CHECK (product_type IS NULL OR product_type IN ('shirt','pants','shoes'))
);
GO
CREATE INDEX IX_sanpham_loai ON sanpham(loai_sp);
CREATE INDEX IX_sanpham_gt   ON sanpham(gt_sp);
CREATE INDEX IX_sanpham_ten  ON sanpham(ten_sp);
GO

/* ---------------------------------------------------------------------
   Bảng đơn hàng
   --------------------------------------------------------------------- */
CREATE TABLE donhang (
    id             INT IDENTITY(1,1) NOT NULL,
    id_nguoidat    INT           NOT NULL,
    tong_tien      DECIMAL(12,0) NOT NULL CONSTRAINT DF_donhang_tongtien DEFAULT 0,
    ngay_dat       DATETIME2(0)  NOT NULL CONSTRAINT DF_donhang_ngaydat DEFAULT SYSDATETIME(),
    trang_thai     VARCHAR(15)   NOT NULL CONSTRAINT DF_donhang_trangthai DEFAULT 'dang_xu_ly',
    ten_nguoinhan  NVARCHAR(100) NULL,
    sdt            VARCHAR(15)   NULL,
    dia_chi        NVARCHAR(255) NULL,
    phuong_thuc_tt VARCHAR(15)   NOT NULL CONSTRAINT DF_donhang_pttt DEFAULT 'cod',
    ghi_chu        NVARCHAR(255) NULL,
    CONSTRAINT PK_donhang PRIMARY KEY (id),
    CONSTRAINT FK_donhang_taikhoan FOREIGN KEY (id_nguoidat) REFERENCES taikhoan(id),
    CONSTRAINT CK_donhang_trangthai CHECK (trang_thai IN ('dang_xu_ly','da_giao','huy')),
    CONSTRAINT CK_donhang_pttt CHECK (phuong_thuc_tt IN ('cod','chuyen_khoan'))
);
GO
CREATE INDEX IX_donhang_nguoidat  ON donhang(id_nguoidat);
CREATE INDEX IX_donhang_trangthai ON donhang(trang_thai);
CREATE INDEX IX_donhang_ngaydat   ON donhang(ngay_dat);
GO

/* ---------------------------------------------------------------------
   Bảng chi tiết đơn hàng
   --------------------------------------------------------------------- */
CREATE TABLE chitietdonhang (
    id         INT IDENTITY(1,1) NOT NULL,
    id_donhang INT           NOT NULL,
    id_sanpham INT           NOT NULL,
    so_luong   INT           NOT NULL CONSTRAINT DF_ct_soluong DEFAULT 1,
    don_gia    DECIMAL(12,0) NOT NULL,           -- giá tại thời điểm đặt hàng
    CONSTRAINT PK_chitietdonhang PRIMARY KEY (id),
    CONSTRAINT FK_ct_donhang FOREIGN KEY (id_donhang) REFERENCES donhang(id) ON DELETE CASCADE,
    CONSTRAINT FK_ct_sanpham FOREIGN KEY (id_sanpham) REFERENCES sanpham(id),
    CONSTRAINT CK_ct_soluong CHECK (so_luong > 0),
    CONSTRAINT CK_ct_dongia CHECK (don_gia >= 0)
);
GO
CREATE INDEX IX_ct_donhang ON chitietdonhang(id_donhang);
CREATE INDEX IX_ct_sanpham ON chitietdonhang(id_sanpham);
GO

/* =====================================================================
   DỮ LIỆU
   ===================================================================== */

-- Đã bỏ ký tự thừa "\r\n" ở cuối hash của admin (làm đăng nhập bị sai).
SET IDENTITY_INSERT taikhoan ON;
INSERT INTO taikhoan (id, name, email, [password], role) VALUES
(1, N'admin',  N'admin@gmail.com',   '$2y$10$7R8Ts4FCQwOwDsuvL1gN2uG9DAcHx1kp8L.ar8rWb3vZff5JeeDdS', N'admin'),
(2, N'Tai',    N'tai27@gmail.com',   '$2y$10$dUEiJSFEkPTxDFGzU4nq6e5DRt1oKk5rkTdPv9ExNTQmwVDP0RsSO', N'admin'),
(3, N'Trân',   N'tran17@gmail.com',  '$2y$10$Npa/J5TtBn6kPOPaUBUnPuFGybDclG449EcUUmtj5qdvzj3Z/HvNC', N'user'),
(4, N'DatTai', N'tai2710@gmail.com', '$2y$10$7R8Ts4FCQwOwDsuvL1gN2uG9DAcHx1kp8L.ar8rWb3vZff5JeeDdS', N'user');
SET IDENTITY_INSERT taikhoan OFF;
GO

SET IDENTITY_INSERT sanpham ON;
INSERT INTO sanpham (id, ten_sp, loai_sp, gt_sp, soluong, gia, hinh_anh, mo_ta, ngay_dang) VALUES
(1,  N'Áo thun đỏ sao vàng', N'Áo', N'Unisex', 10, 50000, '1763397739_vn-11134207-7ra0g-m93336qtp9xj06.png', N'Áo thun đỏ in ngôi sao vàng, chất liệu thoáng mát, phù hợp cả nam và nữ.', '2025-11-17T16:42:19'),
(2,  N'Áo Thun Teelab Grey Trame', N'Áo', N'Unisex', 5, 70000, '1763400445_ao1.png', N'Áo thun màu xám, form rộng thoải mái, dễ phối đồ.', '2025-11-17T17:27:25'),
(3,  N'Áo Sweater Voyage Old Sailor', N'Áo', N'Unisex', 3, 195000, '1763400955_ao2.png', N'Áo sweater dày dặn, giữ ấm tốt, phù hợp thời tiết lạnh.', '2025-11-17T17:35:55'),
(4,  N'Áo Phông Unisex "Minimalist"', N'Áo', N'Unisex', 15, 189000, '1763401289_ao3.jpg', N'Chất liệu cotton 100% thoáng mát, form basic dễ phối đồ, phù hợp cả nam và nữ.', '2025-11-17T17:41:29'),
(5,  N'Quần Jeans Jogger', N'Quần', N'Nam', 23, 40000, '1763401983_quan1.jpg', N'Mềm co giãn The Original 037 Xanh Dương Đậm.', '2025-11-17T17:53:03'),
(6,  N'Quần Jogger Jeans', N'Quần', N'Nam', 32, 37000, '1763402065_quan2.jpg', N'Co giãn Multi Color Jean 010 Xám.', '2025-11-17T17:54:25'),
(7,  N'Quần Short Jean', N'Quần', N'Nam', 12, 33000, '1763402232_quan3.jpg', N'Lưng thun dễ chịu The Original 041 Xanh Dương Nhạt.', '2025-11-17T17:57:12'),
(8,  N'Đầm dáng dài sọc phối ren', N'Váy', N'Nữ', 3, 40000, '1763402464_vay1.jpg', N'Đầm dáng dài sọc phối ren, có size lớn.', '2025-11-17T18:01:04'),
(9,  N'Bộ Đồ Nữ Mặc Nhà Phối Màu', N'Đồ bộ', N'Nữ', 41, 50000, '1763402704_dobo1.jpg', N'Bộ đồ mặc nhà thoải mái, phối màu trẻ trung.', '2025-11-17T18:05:04'),
(10, N'Bộ Đồ Nữ Dài Tay Dáng Suông Phối Màu', N'Đồ bộ', N'Nữ', 30, 55000, '1763402771_dobo2.jpg', N'Dáng suông dễ phối màu, tay dài.', '2025-11-17T18:06:11'),
(11, N'Quần Jeans Suông Gai Xanh', N'Quần', N'Nữ', 20, 50000, '1763402845_quan4.jpg', N'Quần jeans ống suông màu xanh, chất vải dày.', '2025-11-17T18:07:25'),
(12, N'Adidas Superstar André Saraiva Chalk', N'Giày dép', N'Nam', 25, 850000, '1763527142_giay1.jpg', N'Giày Adidas Superstar phiên bản André Saraiva, kiểu dáng cổ điển.', '2025-11-19T04:39:02'),
(13, N'Giày Thể Thao Nam MWC 5857', N'Giày dép', N'Nam', 7, 345000, '1763531750_giay2.jpg', N'Giày da thể thao nam đi học, đi chơi, leo núi, chạy bộ, bền đẹp, trẻ trung, năng động.', '2025-11-19T05:55:50'),
(14, N'Áo thun đen lưu niệm của WinWinStore', N'Áo', N'Nam', 14, 30000, '1763553470_ao4.jpg', N'Thiết kế đơn giản, dễ mặc, phù hợp cho cả nam và nữ. Chất liệu 100% cotton mềm, thoáng khí. Logo "W" nhỏ in ở ngực trái, mặt sau in logo các thương hiệu hợp tác cùng WinWinStore.', '2025-11-19T11:57:50'),
(27, N'Áo Thun Waffle Thoáng Mát Non Branded 001 Đen', N'Áo', N'Nam', 9, 150000, '1763624872_ao6.jpg', N'Áo thun waffle thoáng mát, màu đen, dễ phối đồ.', '2025-11-20T07:47:52');
SET IDENTITY_INSERT sanpham OFF;
GO

SET IDENTITY_INSERT donhang ON;
INSERT INTO donhang (id, id_nguoidat, tong_tien, ngay_dat, trang_thai) VALUES
(2,  2, 1195000, '2025-11-19T06:08:49', 'da_giao'),
(3,  3, 345000,  '2025-11-19T11:38:48', 'da_giao'),
(4,  3, 30000,   '2025-11-19T11:58:33', 'huy'),
(5,  3, 30000,   '2025-11-19T15:38:27', 'da_giao'),
(6,  3, 345000,  '2025-11-19T15:45:55', 'da_giao'),
(7,  2, 30000,   '2025-11-19T16:15:03', 'da_giao'),
(8,  2, 850000,  '2025-11-19T16:23:00', 'da_giao'),
(9,  3, 30000,   '2025-11-19T16:39:54', 'da_giao'),
(10, 2, 345000,  '2025-11-19T17:05:38', 'da_giao'),
(11, 2, 30000,   '2025-11-19T17:31:47', 'da_giao'),
(12, 2, 550000,  '2025-11-19T17:35:16', 'da_giao'),
(13, 2, 850000,  '2025-11-20T07:15:26', 'da_giao'),
(14, 2, 850000,  '2025-11-20T07:24:42', 'da_giao'),
(15, 2, 880000,  '2025-11-20T07:33:26', 'da_giao'),
(16, 1, 150000,  '2025-11-20T07:48:49', 'da_giao'),
(17, 1, 50000,   '2025-11-20T08:07:14', 'da_giao'),
(18, 2, 1730000, '2025-11-20T08:23:11', 'da_giao'),
(19, 1, 495000,  '2025-11-21T01:28:22', 'da_giao'),
(20, 1, 150000,  '2025-11-21T01:29:22', 'da_giao');
SET IDENTITY_INSERT donhang OFF;
GO

SET IDENTITY_INSERT chitietdonhang ON;
INSERT INTO chitietdonhang (id, id_donhang, id_sanpham, so_luong, don_gia) VALUES
(4,  2, 13, 1, 345000),
(5,  2, 12, 1, 850000),
(6,  3, 13, 1, 345000),
(7,  4, 14, 1, 30000),
(8,  5, 14, 1, 30000),
(9,  6, 13, 1, 345000),
(10, 7, 14, 1, 30000),
(11, 8, 12, 1, 850000),
(12, 9, 14, 1, 30000),
(13, 10, 13, 1, 345000),
(14, 11, 14, 1, 30000),
(15, 12, 1, 11, 50000),
(16, 13, 12, 1, 850000),
(17, 14, 12, 1, 850000),
(18, 15, 12, 1, 850000),
(19, 15, 14, 1, 30000),
(20, 16, 27, 1, 150000),
(21, 17, 11, 1, 50000),
(22, 18, 12, 2, 850000),
(23, 18, 14, 1, 30000),
(24, 19, 27, 1, 150000),
(25, 19, 13, 1, 345000),
(26, 20, 27, 1, 150000);
SET IDENTITY_INSERT chitietdonhang OFF;
GO

/* =====================================================================
   VIEW hỗ trợ báo cáo / thống kê (dùng cho trang admin)
   ===================================================================== */

-- Chi tiết đơn hàng kèm tên người đặt, tên sản phẩm, thành tiền
CREATE VIEW v_chitiet_donhang AS
SELECT d.id AS id_donhang, d.ngay_dat, d.trang_thai, t.name AS nguoi_dat,
       s.id AS id_sanpham, s.ten_sp, c.so_luong, c.don_gia,
       (c.so_luong * c.don_gia) AS thanh_tien
FROM donhang d
JOIN taikhoan t ON t.id = d.id_nguoidat
JOIN chitietdonhang c ON c.id_donhang = d.id
JOIN sanpham s ON s.id = c.id_sanpham;
GO

-- Doanh thu theo ngày (không tính đơn đã hủy)
CREATE VIEW v_doanhthu_ngay AS
SELECT CAST(ngay_dat AS DATE) AS ngay, COUNT(*) AS so_don, SUM(tong_tien) AS doanh_thu
FROM donhang
WHERE trang_thai <> 'huy'
GROUP BY CAST(ngay_dat AS DATE);
GO

-- Sản phẩm bán chạy (không tính đơn đã hủy).
-- Khi dùng, sắp xếp ở câu truy vấn: SELECT * FROM v_sp_banchay ORDER BY da_ban DESC;
CREATE VIEW v_sp_banchay AS
SELECT s.id, s.ten_sp, SUM(c.so_luong) AS da_ban
FROM chitietdonhang c
JOIN donhang d ON d.id = c.id_donhang AND d.trang_thai <> 'huy'
JOIN sanpham s ON s.id = c.id_sanpham
GROUP BY s.id, s.ten_sp;
GO

/* =====================================================================
   THỦ TỤC ĐẶT HÀNG MẪU: kiểm tra tồn kho + trừ kho trong 1 transaction
   Dùng: EXEC sp_TruKho @id_sanpham = 1, @so_luong = 2;
   ===================================================================== */
CREATE PROCEDURE sp_TruKho
    @id_sanpham INT,
    @so_luong   INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Trừ kho chỉ khi còn đủ hàng; nếu không đủ thì không cập nhật dòng nào
    UPDATE sanpham
       SET soluong = soluong - @so_luong
     WHERE id = @id_sanpham AND soluong >= @so_luong;

    IF @@ROWCOUNT = 0
        THROW 50001, N'Sản phẩm không đủ số lượng trong kho.', 1;
END
GO
