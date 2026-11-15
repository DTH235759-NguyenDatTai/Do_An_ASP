using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class Chitietdonhang
{
    public int Id { get; set; }

    public int IdDonhang { get; set; }

    public int IdSanpham { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual Donhang IdDonhangNavigation { get; set; } = null!;

    public virtual Sanpham IdSanphamNavigation { get; set; } = null!;
}
