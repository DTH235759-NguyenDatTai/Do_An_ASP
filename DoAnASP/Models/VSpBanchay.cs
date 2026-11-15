using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class VSpBanchay
{
    public int Id { get; set; }

    public string TenSp { get; set; } = null!;

    public int? DaBan { get; set; }
}
