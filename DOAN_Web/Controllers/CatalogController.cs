using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;

namespace DOAN_Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/danh-muc/{slug}")]
        public async Task<IActionResult> Category(string slug, int page = 1)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug);

            if (category == null)
            {
                return NotFound();
            }

            int pageSize = 12;
            var query = _context.Products
                .Include(p => p.Author)
                .Include(p => p.ProductCategories)
                .Where(p => p.Status == "Active" && 
                           p.ProductCategories.Any(pc => pc.CategoryId == category.CategoryId))
                .OrderByDescending(p => p.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Category = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(products);
        }

        [HttpGet("/danh-muc")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ProductCategories)
                    .ThenInclude(pc => pc.Product)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }
    }
}
