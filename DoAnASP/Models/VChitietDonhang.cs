using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class VChitietDonhang
{
    public int IdDonhang { get; set; }

    public DateTime NgayDat { get; set; }

    public string TrangThai { get; set; } = null!;

    public string NguoiDat { get; set; } = null!;

    public int IdSanpham { get; set; }

    public string TenSp { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? ThanhTien { get; set; }
}
