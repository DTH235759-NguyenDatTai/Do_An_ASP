using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class VDoanhthuNgay
{
    public DateOnly? Ngay { get; set; }

    public int? SoDon { get; set; }

    public decimal? DoanhThu { get; set; }
}
