using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnASP.Models;
using System.Collections.Generic;
using System.Linq;

namespace DoAnASP.Controllers
{
    public class CartController : Controller
    {
        private readonly QlquanaoContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(QlquanaoContext context)
        {
            _context = context;
        }

        // GET: Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Sanphams.FindAsync(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            if (product.Soluong < quantity)
            {
                return Json(new { success = false, message = "Số lượng sản phẩm không đủ" });
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.Soluong)
                {
                    existingItem.Quantity = product.Soluong;
                }
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.TenSp ?? string.Empty,
                    Price = product.Gia,
                    ImageUrl = product.HinhAnh ?? string.Empty,
                    Quantity = quantity
                });
            }

            SaveCart(cart);

            var cartCount = cart.Sum(c => c.Quantity);

            return Json(new
            {
                success = true,
                message = $"Đã thêm {product.TenSp} vào giỏ hàng!",
                cartCount = cartCount
            });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }

            var cartCount = cart.Sum(c => c.Quantity);
            var total = cart.Sum(c => c.Price * c.Quantity);

            return Json(new
            {
                success = true,
                cartCount = cartCount,
                total = total
            });
        }

        // POST: Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            var cartCount = cart.Sum(c => c.Quantity);

            return Json(new
            {
                success = true,
                cartCount = cartCount
            });
        }

        // GET: Cart/ViewCart (for navbar partial)
        public IActionResult ViewCart()
        {
            var cart = GetCart();
            return PartialView("_CartPartial", cart);
        }

        // GET: Cart/GetCartItems (for layout partial)
        public List<CartItem> GetCartItems()
        {
            return GetCart();
        }

        // GET: Cart/GetCartCount
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            var count = cart.Sum(c => c.Quantity);
            return Json(new { count = count });
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}