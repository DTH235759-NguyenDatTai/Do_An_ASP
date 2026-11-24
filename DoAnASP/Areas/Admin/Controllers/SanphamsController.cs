using DoAnASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnASP.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class SanphamsController : Controller
    {
        private readonly QlquanaoContext _context;
        private readonly IWebHostEnvironment _env;

        public SanphamsController(QlquanaoContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Sanphams
        public async Task<IActionResult> Index(string searchString, string loaiSp, int? page)
        {
            var sanphams = _context.Sanphams.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(s => s.TenSp.Contains(searchString) || s.LoaiSp.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(loaiSp))
            {
                sanphams = sanphams.Where(s => s.LoaiSp == loaiSp);
            }

            ViewBag.SearchString = searchString;
            ViewBag.LoaiSp = loaiSp;
            ViewBag.LoaiSpList = await _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToListAsync();

            int pageSize = 10;
            int pageNumber = page ?? 1;

            return View(await PaginatedList<Sanpham>.CreateAsync(sanphams.OrderByDescending(s => s.NgayDang), pageNumber, pageSize));
        }

        // GET: Admin/Sanphams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // GET: Admin/Sanphams/Create
        public IActionResult Create()
        {
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View();
        }

        // POST: Admin/Sanphams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType")] Sanpham sanpham, IFormFile? HinhAnhFile, IFormFile? TryonImageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle main image upload
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    sanpham.HinhAnh = await SaveImageAsync(HinhAnhFile);
                }

                // Handle tryon image upload
                if (TryonImageFile != null && TryonImageFile.Length > 0)
                {
                    sanpham.TryonImage = await SaveImageAsync(TryonImageFile);
                }

                sanpham.NgayDang = DateTime.Now;
                _context.Add(sanpham);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // GET: Admin/Sanphams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // POST: Admin/Sanphams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,TenSp,LoaiSp,GtSp,Soluong,Gia,HinhAnh,MoTa,NgayDang,TryonImage,ProductType,TryonType")] Sanpham sanpham, IFormFile? HinhAnhFile, IFormFile? TryonImageFile)
        {
            if (id != sanpham.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing product to preserve images if not uploading new ones
                    var existingProduct = await _context.Sanphams.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Handle main image upload
                    if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                    {
                        // Delete old image if it's a local file
                        if (!string.IsNullOrEmpty(existingProduct.HinhAnh) && existingProduct.HinhAnh.StartsWith("/images/"))
                        {
                            DeleteImage(existingProduct.HinhAnh);
                        }
                        sanpham.HinhAnh = await SaveImageAsync(HinhAnhFile);
                    }
                    else
                    {
                        // Keep existing image
                        sanpham.HinhAnh = existingProduct.HinhAnh;
                    }

                    // Handle tryon image upload
                    if (TryonImageFile != null && TryonImageFile.Length > 0)
                    {
                        // Delete old tryon image if it's a local file
                        if (!string.IsNullOrEmpty(existingProduct.TryonImage) && existingProduct.TryonImage.StartsWith("/images/"))
                        {
                            DeleteImage(existingProduct.TryonImage);
                        }
                        sanpham.TryonImage = await SaveImageAsync(TryonImageFile);
                    }
                    else
                    {
                        // Keep existing tryon image
                        sanpham.TryonImage = existingProduct.TryonImage;
                    }

                    // Preserve original NgayDang
                    sanpham.NgayDang = existingProduct.NgayDang;

                    _context.Update(sanpham);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanphamExists(sanpham.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiSpList = _context.Sanphams.Select(s => s.LoaiSp).Distinct().ToList();
            return View(sanpham);
        }

        // GET: Admin/Sanphams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // POST: Admin/Sanphams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham != null)
            {
                // Check if product has order details (soft delete approach)
                bool hasOrders = await _context.Chitietdonhangs.AnyAsync(c => c.IdSanpham == id);
                if (hasOrders)
                {
                    // Soft delete: mark as hidden instead of actually deleting
                    sanpham.Soluong = 0;
                    sanpham.MoTa = "[Đã ngưng bán] " + (sanpham.MoTa ?? "");
                    _context.Update(sanpham);
                    TempData["SuccessMessage"] = "Sản phẩm đã có đơn hàng, đã ẩn sản phẩm (soft delete)!";
                }
                else
                {
                    // Hard delete: remove images and product
                    if (!string.IsNullOrEmpty(sanpham.HinhAnh) && sanpham.HinhAnh.StartsWith("/images/"))
                    {
                        DeleteImage(sanpham.HinhAnh);
                    }
                    if (!string.IsNullOrEmpty(sanpham.TryonImage) && sanpham.TryonImage.StartsWith("/images/"))
                    {
                        DeleteImage(sanpham.TryonImage);
                    }
                    _context.Sanphams.Remove(sanpham);
                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif, webp");
            }

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("Kích thước file không được vượt quá 5MB");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(_env.WebRootPath, "images", fileName);

            // Save file
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }

        private void DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch
            {
                // Ignore delete errors
            }
        }

        private bool SanphamExists(int? id)
        {
            return _context.Sanphams.Any(e => e.Id == id);
        }
    }

    // Helper class for pagination
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}