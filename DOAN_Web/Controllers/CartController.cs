using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;
using DOAN_Web.Models;
using DOAN_Web.ViewModels;
using System.Text.Json;

namespace DOAN_Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string CartSessionKey = "ShoppingCart";

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("/gio-hang")]
        public async Task<IActionResult> Index()
        {
            var cart = await GetCartAsync();
            return View(cart);
        }

        [HttpPost("/cart/them")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            
            if (product == null || product.Status != "Active")
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            if (product.StockQty < request.Quantity)
            {
                return Json(new { success = false, message = "Không đủ hàng trong kho" });
            }

            var cart = await GetCartItemsAsync();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = product.ProductId,
                    Title = product.Title,
                    CoverImageUrl = product.CoverImageUrl,
                    Price = product.Price,
                    Quantity = request.Quantity
                });
            }

            await SaveCartAsync(cart);

            return Json(new 
            { 
                success = true, 
                message = "Đã thêm vào giỏ hàng",
                cartCount = cart.Sum(c => c.Quantity)
            });
        }

        [HttpPost("/cart/cap-nhat")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var cart = await GetCartItemsAsync();
            var item = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (item == null)
            {
                return Json(new { success = false, message = "Sản phẩm không có trong giỏ hàng" });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || request.Quantity > product.StockQty)
            {
                return Json(new { success = false, message = "Không đủ hàng trong kho" });
            }

            item.Quantity = request.Quantity;
            await SaveCartAsync(cart);

            var cartViewModel = new CartViewModel { Items = cart };

            return Json(new 
            { 
                success = true,
                subtotal = item.Subtotal,
                total = cartViewModel.Total,
                cartCount = cart.Sum(c => c.Quantity)
            });
        }

        [HttpPost("/cart/xoa/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = await GetCartItemsAsync();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                await SaveCartAsync(cart);
            }

            return Json(new 
            { 
                success = true,
                cartCount = cart.Sum(c => c.Quantity)
            });
        }

        [HttpGet("/cart/count")]
        public async Task<IActionResult> GetCartCount()
        {
            var cart = await GetCartItemsAsync();
            return Json(new { count = cart.Sum(c => c.Quantity) });
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user != null)
            {
                // Load from database for logged-in users
                var cartItems = await _context.CartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                return cartItems.Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Title = c.Product.Title,
                    CoverImageUrl = c.Product.CoverImageUrl,
                    Price = c.Product.Price,
                    Quantity = c.Quantity
                }).ToList();
            }
            else
            {
                // Load from session for guests
                var cartJson = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<CartItemViewModel>();
                }

                return JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson) 
                       ?? new List<CartItemViewModel>();
            }
        }

        private async Task SaveCartAsync(List<CartItemViewModel> cart)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Save to database for logged-in users
                var existingCartItems = await _context.CartItems
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                _context.CartItems.RemoveRange(existingCartItems);

                foreach (var item in cart)
                {
                    _context.CartItems.Add(new CartItem
                    {
                        UserId = user.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        AddedAt = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                // Save to session for guests
                var cartJson = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(CartSessionKey, cartJson);
            }
        }

        private async Task<CartViewModel> GetCartAsync()
        {
            var items = await GetCartItemsAsync();
            return new CartViewModel { Items = items };
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
