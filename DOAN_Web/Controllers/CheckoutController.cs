using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;
using DOAN_Web.Models;
using DOAN_Web.ViewModels;
using System.Text.Json;

namespace DOAN_Web.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private const string CartSessionKey = "ShoppingCart";

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet("/thanh-toan")]
        public async Task<IActionResult> Index()
        {
            var cart = await GetCartItemsAsync();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                CustomerName = user?.FullName ?? "",
                CustomerPhone = user?.PhoneNumber ?? ""
            };

            ViewBag.Cart = new CartViewModel { Items = cart };
            return View(model);
        }

        [HttpPost("/thanh-toan/dat-hang")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cart = await GetCartItemsAsync();
                ViewBag.Cart = new CartViewModel { Items = cart };
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            var cartItems = await GetCartItemsAsync();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "Cart");
            }

            // Check stock availability
            foreach (var cartItem in cartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null || product.StockQty < cartItem.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm '{cartItem.Title}' không đủ hàng trong kho";
                    return RedirectToAction("Index");
                }
            }

            // Create order
            var subtotal = cartItems.Sum(c => c.Subtotal);
            var shippingFee = 30000m; // Fixed shipping fee
            var total = subtotal + shippingFee;

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = user.Id,
                Status = "ChoThanhToan",
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Total = total,
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                ShippingAddress = $"{model.AddressLine1}, {model.AddressLine2}, {model.Ward}, {model.District}, {model.City}",
                Notes = model.Notes,
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items and update stock
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price
                };
                _context.OrderItems.Add(orderItem);

                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.StockQty -= cartItem.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            // Clear cart
            await ClearCartAsync();

            return RedirectToAction("PaymentInstructions", new { id = order.OrderId });
        }

        [HttpGet("/don-hang/{id}/thanh-toan")]
        public async Task<IActionResult> PaymentInstructions(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (order.UserId != user?.Id)
            {
                return Forbid();
            }

            return View(order);
        }

        [HttpPost("/don-hang/{id}/xac-nhan-thanh-toan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPaymentProof(int id, IFormFile paymentImage)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (order.UserId != user?.Id)
            {
                return Forbid();
            }

            if (paymentImage == null || paymentImage.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ảnh chuyển khoản";
                return RedirectToAction("PaymentInstructions", new { id });
            }

            // Validate file type and size
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(paymentImage.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] = "Chỉ chấp nhận file ảnh định dạng JPG, JPEG, PNG";
                return RedirectToAction("PaymentInstructions", new { id });
            }

            if (paymentImage.Length > 5 * 1024 * 1024) // 5MB
            {
                TempData["Error"] = "Kích thước file không được vượt quá 5MB";
                return RedirectToAction("PaymentInstructions", new { id });
            }

            // Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "payments");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await paymentImage.CopyToAsync(fileStream);
            }

            // Create or update payment proof
            var proof = await _context.PaymentProofs.FirstOrDefaultAsync(p => p.OrderId == id);
            if (proof == null)
            {
                proof = new PaymentProof
                {
                    OrderId = id,
                    ImageUrl = $"/images/payments/{fileName}",
                    UploadedAt = DateTime.Now
                };
                _context.PaymentProofs.Add(proof);
            }
            else
            {
                proof.ImageUrl = $"/images/payments/{fileName}";
                proof.UploadedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã tải lên ảnh chuyển khoản thành công. Chúng tôi sẽ xác nhận trong thời gian sớm nhất.";
            return RedirectToAction("DonHangCuaToi", "Account");
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
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
                var cartJson = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<CartItemViewModel>();
                }

                return JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson)
                       ?? new List<CartItemViewModel>();
            }
        }

        private async Task ClearCartAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
            else
            {
                HttpContext.Session.Remove(CartSessionKey);
            }
        }

        private string GenerateOrderNumber()
        {
            var today = DateTime.Now;
            var orderCount = _context.Orders
                .Count(o => o.CreatedAt.Date == today.Date);

            return $"ORD{today:yyyyMMdd}{(orderCount + 1):D3}";
        }
    }
}
