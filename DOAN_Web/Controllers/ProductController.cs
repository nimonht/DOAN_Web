using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;

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

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
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
