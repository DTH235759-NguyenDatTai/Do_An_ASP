using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnASP.Models;

public partial class Sanpham
{
    public int Id { get; set; }

    [Display(Name = "Tên sản phẩm")]
    [Required]
    public string TenSp { get; set; } = null!;

    [Display(Name = "Loại sản phẩm")]
    [Required]
    public string LoaiSp { get; set; } = null!;

    [Display(Name = "Giới tính phù hợp với sản phẩm")]
    [Required]
    public string GtSp { get; set; } = null!;

    [Display(Name = "Số lượng")]
    [Required]
    public int Soluong { get; set; }

    [Display(Name = "Giá")]
    [Required]
    public decimal Gia { get; set; }

    [Display(Name = "Hình ảnh")]
    [Required]
    public string? HinhAnh { get; set; }

    [Display(Name = "Mô tả")]
    [Required]
    public string? MoTa { get; set; }

    public DateTime NgayDang { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();
}
