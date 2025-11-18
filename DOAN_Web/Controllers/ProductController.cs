using System.Security.Claims;
using DOAN_Web.Data;
using DOAN_Web.Models;
using DOAN_Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOAN_Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/sach/{slug}")]
        public async Task<IActionResult> Detail(string slug)
        {
            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products from the same category
            var categoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
            var relatedProducts = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.ProductCategories)
                .Where(p => p.ProductId != product.ProductId && 
                           p.Status == "Active" &&
                           p.ProductCategories.Any(pc => categoryIds.Contains(pc.CategoryId)))
                .Take(6)
                .ToListAsync();

            var orderedReviews = product.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var averageRating = orderedReviews.Any()
                ? orderedReviews.Average(r => r.Rating)
                : 0d;

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.OrderedReviews = orderedReviews;
            ViewBag.AverageRating = averageRating;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userReview = orderedReviews.FirstOrDefault(r => r.UserId == userId);

                var hasCompletedOrder = await _context.OrderItems
                    .AnyAsync(oi => oi.Order.UserId == userId &&
                                    oi.Order.Status == "HoanThanh" &&
                                    oi.ProductId == product.ProductId);

                ViewBag.HasPurchased = hasCompletedOrder;
                ViewBag.UserReview = userReview;
                ViewBag.CanReview = hasCompletedOrder && userReview == null;
            }
            else
            {
                ViewBag.HasPurchased = false;
                ViewBag.UserReview = null;
                ViewBag.CanReview = false;
            }

            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewInputModel input)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == input.ProductId);

            if (product == null)
            {
                TempData["ReviewError"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index", "Home");
            }

            var redirectUrl = $"/sach/{product.Slug}#reviews";

            if (!ModelState.IsValid)
            {
                TempData["ReviewError"] = "Vui lòng cung cấp thông tin đánh giá hợp lệ.";
                return Redirect(redirectUrl);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "Account", new { returnUrl = $"/sach/{product.Slug}" });
            }

            var hasCompletedOrder = await _context.OrderItems
                .AnyAsync(oi => oi.Order.UserId == userId &&
                                oi.Order.Status == "HoanThanh" &&
                                oi.ProductId == input.ProductId);

            if (!hasCompletedOrder)
            {
                TempData["ReviewError"] = "Bạn chỉ có thể đánh giá sau khi đơn hàng được hoàn thành.";
                return Redirect(redirectUrl);
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == input.ProductId && r.UserId == userId);

            if (existingReview != null)
            {
                TempData["ReviewError"] = "Bạn đã đánh giá sản phẩm này.";
                return Redirect(redirectUrl);
            }

            var review = new Review
            {
                ProductId = input.ProductId,
                UserId = userId,
                Rating = input.Rating,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["ReviewSuccess"] = "Cảm ơn bạn đã gửi đánh giá.";
            return Redirect(redirectUrl);
        }

        [HttpGet("/tim-kiem")]
        public async Task<IActionResult> Search(string? q, int? minPrice, int? maxPrice, int? categoryId, string? sortBy, int page = 1)
        {
            var query = _context.Products
                .Include(p => p.Author)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.Status == "Active");

            // Search by keyword
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => p.Title.Contains(q) || 
                                        p.Description.Contains(q) ||
                                        p.Author.Name.Contains(q));
                ViewBag.SearchQuery = q;
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));
                ViewBag.SelectedCategoryId = categoryId.Value;
            }

            // Sort
            query = sortBy switch
            {
                "price-asc" => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "name-asc" => query.OrderBy(p => p.Title),
                "name-desc" => query.OrderByDescending(p => p.Title),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            int pageSize = 12;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortBy = sortBy;

            return View(products);
        }
    }
}
