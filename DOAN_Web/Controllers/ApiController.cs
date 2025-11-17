using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;

namespace DOAN_Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.ProductCategories)
                    .ThenInclude(pc => pc.Product)
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.CategoryId,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.BackgroundImageUrl,
                    ProductCount = c.ProductCategories.Count(pc => pc.Product != null && pc.Product.Status == "Active")
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}
