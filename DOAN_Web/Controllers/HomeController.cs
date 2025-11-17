using System.Diagnostics;
using DOAN_Web.Models;
using DOAN_Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOAN_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured products (newest products)
            var featuredProducts = await _context.Products
                .Include(p => p.Author)
                .Where(p => p.Status == "Active")
                .OrderByDescending(p => p.CreatedAt)
                .Take(12)
                .ToListAsync();

            // Get categories 
            var categories = await _context.Categories
                .Include(c => c.ProductCategories)
                    .ThenInclude(pc => pc.Product)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            
            return View(featuredProducts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
