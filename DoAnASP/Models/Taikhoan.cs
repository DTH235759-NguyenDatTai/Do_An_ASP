using System;
using System.Collections.Generic;

namespace DoAnASP.Models;

public partial class Taikhoan
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
