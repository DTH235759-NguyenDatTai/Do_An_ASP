using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class Sanpham
{
    public int Id { get; set; }

    public string TenSp { get; set; } = null!;

    public string LoaiSp { get; set; } = null!;

    public string GtSp { get; set; } = null!;

    public int Soluong { get; set; }

    public decimal Gia { get; set; }

    public string? HinhAnh { get; set; }

    public string? MoTa { get; set; }

    public DateTime NgayDang { get; set; }

    public string? TryonImage { get; set; }

    public string? ProductType { get; set; }

    public string? TryonType { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();
}
