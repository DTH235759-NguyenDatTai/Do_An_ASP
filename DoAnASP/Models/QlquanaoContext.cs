using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Models;

public partial class QlquanaoContext : DbContext
{
    public QlquanaoContext()
    {
    }

    public QlquanaoContext(DbContextOptions<QlquanaoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chitietdonhang> Chitietdonhangs { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<VChitietDonhang> VChitietDonhangs { get; set; }

    public virtual DbSet<VDoanhthuNgay> VDoanhthuNgays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<Chitietdonhang>(entity =>
        {
            entity.ToTable("chitietdonhang");

            entity.HasIndex(e => e.IdDonhang, "IX_ct_donhang");

            entity.HasIndex(e => e.IdSanpham, "IX_ct_sanpham");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("don_gia");
            entity.Property(e => e.IdDonhang).HasColumnName("id_donhang");
            entity.Property(e => e.IdSanpham).HasColumnName("id_sanpham");
            entity.Property(e => e.SoLuong)
                .HasDefaultValue(1, "DF_ct_soluong")
                .HasColumnName("so_luong");

            entity.HasOne(d => d.IdDonhangNavigation).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.IdDonhang)
                .HasConstraintName("FK_ct_donhang");

            entity.HasOne(d => d.IdSanphamNavigation).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.IdSanpham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ct_sanpham");
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.ToTable("donhang");

            entity.HasIndex(e => e.NgayDat, "IX_donhang_ngaydat");

            entity.HasIndex(e => e.IdNguoidat, "IX_donhang_nguoidat");

            entity.HasIndex(e => e.TrangThai, "IX_donhang_trangthai");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("dia_chi");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(255)
                .HasColumnName("ghi_chu");
            entity.Property(e => e.IdNguoidat).HasColumnName("id_nguoidat");
            entity.Property(e => e.NgayDat)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_donhang_ngaydat")
                .HasColumnName("ngay_dat");
            entity.Property(e => e.PhuongThucTt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("cod", "DF_donhang_pttt")
                .HasColumnName("phuong_thuc_tt");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
            entity.Property(e => e.TenNguoinhan)
                .HasMaxLength(100)
                .HasColumnName("ten_nguoinhan");
            entity.Property(e => e.TongTien)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("tong_tien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("dang_xu_ly", "DF_donhang_trangthai")
                .HasColumnName("trang_thai");

            entity.HasOne(d => d.IdNguoidatNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.IdNguoidat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_donhang_taikhoan");
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.ToTable("sanpham");

            entity.HasIndex(e => e.GtSp, "IX_sanpham_gt");

            entity.HasIndex(e => e.LoaiSp, "IX_sanpham_loai");

            entity.HasIndex(e => e.TenSp, "IX_sanpham_ten");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gia)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("gia");
            entity.Property(e => e.GtSp)
                .HasMaxLength(20)
                .HasDefaultValue("Unisex", "DF_sanpham_gt")
                .HasColumnName("gt_sp");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("hinh_anh");
            entity.Property(e => e.LoaiSp)
                .HasMaxLength(50)
                .HasColumnName("loai_sp");
            entity.Property(e => e.MoTa).HasColumnName("mo_ta");
            entity.Property(e => e.NgayDang)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_sanpham_ngaydang")
                .HasColumnName("ngay_dang");
            entity.Property(e => e.Soluong).HasColumnName("soluong");
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("ten_sp");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.ToTable("taikhoan");

            entity.HasIndex(e => e.Email, "UQ_taikhoan_email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("dia_chi");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NgayTao)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())", "DF_taikhoan_ngaytao")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasDefaultValue("user", "DF_taikhoan_role")
                .HasColumnName("role");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("sdt");
        });

        modelBuilder.Entity<VChitietDonhang>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_chitiet_donhang");

            entity.Property(e => e.DonGia)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("don_gia");
            entity.Property(e => e.IdDonhang).HasColumnName("id_donhang");
            entity.Property(e => e.IdSanpham).HasColumnName("id_sanpham");
            entity.Property(e => e.NgayDat)
                .HasPrecision(0)
                .HasColumnName("ngay_dat");
            entity.Property(e => e.NguoiDat)
                .HasMaxLength(100)
                .HasColumnName("nguoi_dat");
            entity.Property(e => e.SoLuong).HasColumnName("so_luong");
            entity.Property(e => e.TenSp)
                .HasMaxLength(150)
                .HasColumnName("ten_sp");
            entity.Property(e => e.ThanhTien)
                .HasColumnType("decimal(23, 0)")
                .HasColumnName("thanh_tien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("trang_thai");
        });

        modelBuilder.Entity<VDoanhthuNgay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_doanhthu_ngay");

            entity.Property(e => e.DoanhThu)
                .HasColumnType("decimal(38, 0)")
                .HasColumnName("doanh_thu");
            entity.Property(e => e.Ngay).HasColumnName("ngay");
            entity.Property(e => e.SoDon).HasColumnName("so_don");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
