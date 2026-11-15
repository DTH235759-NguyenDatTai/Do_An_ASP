using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class Donhang
{
    public int Id { get; set; }

    public int IdNguoidat { get; set; }

    public decimal TongTien { get; set; }

    public DateTime NgayDat { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? TenNguoinhan { get; set; }

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public string PhuongThucTt { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    public virtual Taikhoan IdNguoidatNavigation { get; set; } = null!;
}
