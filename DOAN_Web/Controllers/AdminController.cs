using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Data;
using DOAN_Web.Models;

namespace DOAN_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("/admin")]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/admin/dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "ChoThanhToan");
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "HoanThanh")
                .SumAsync(o => o.Total);
            var totalProducts = await _context.Products.CountAsync();
            var activeProducts = await _context.Products.CountAsync(p => p.Status == "Active");

            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.ActiveProducts = activeProducts;

            // Monthly revenue for chart
            var monthlyRevenue = new decimal[12];
            for (int i = 0; i < 12; i++)
            {
                var month = i + 1;
                var revenue = await _context.Orders
                    .Where(o => o.Status == "HoanThanh" && 
                               o.CreatedAt.Year == DateTime.Now.Year && 
                               o.CreatedAt.Month == month)
                    .SumAsync(o => o.Total);
                monthlyRevenue[i] = revenue;
            }
            ViewBag.MonthlyRevenue = System.Text.Json.JsonSerializer.Serialize(monthlyRevenue);

            // Order status distribution
            var orderStatusData = new[]
            {
                await _context.Orders.CountAsync(o => o.Status == "ChoThanhToan"),
                await _context.Orders.CountAsync(o => o.Status == "DaXacNhan"),
                await _context.Orders.CountAsync(o => o.Status == "DangGiao"),
                await _context.Orders.CountAsync(o => o.Status == "HoanThanh"),
                await _context.Orders.CountAsync(o => o.Status == "DaHuy")
            };
            ViewBag.OrderStatusData = System.Text.Json.JsonSerializer.Serialize(orderStatusData);

            // Top selling products
            var topProducts = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    SoldCount = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(5)
                .ToListAsync();

            var topProductsList = new List<dynamic>();
            foreach (var tp in topProducts)
            {
                var product = await _context.Products
                    .Include(p => p.Author)
                    .FirstOrDefaultAsync(p => p.ProductId == tp.ProductId);
                if (product != null)
                {
                    topProductsList.Add(new
                    {
                        product.ProductId,
                        product.Title,
                        product.CoverImageUrl,
                        Author = product.Author,
                        SoldCount = tp.SoldCount
                    });
                }
            }
            ViewBag.TopProducts = topProductsList;

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.PaymentProof)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(recentOrders);
        }

        [HttpGet("/admin/don-hang")]
        public async Task<IActionResult> Orders(string? status)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.PaymentProof)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
                ViewBag.SelectedStatus = status;
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet("/admin/don-hang/{id}")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.PaymentProof)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost("/admin/don-hang/{id}/xac-nhan-thanh-toan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPayment(int id)
        {
            var order = await _context.Orders
                .Include(o => o.PaymentProof)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.PaymentProof == null)
            {
                TempData["Error"] = "Chưa có chứng từ thanh toán";
                return RedirectToAction("OrderDetail", new { id });
            }

            var user = await _userManager.GetUserAsync(User);
            
            order.Status = "DaXacNhan";
            order.UpdatedAt = DateTime.Now;
            order.PaymentProof.VerifiedBy = user?.Id;
            order.PaymentProof.VerifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xác nhận thanh toán thành công";
            return RedirectToAction("OrderDetail", new { id });
        }

        [HttpPost("/admin/don-hang/{id}/cap-nhat-trang-thai")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "ChoThanhToan", "DaXacNhan", "DangGiao", "HoanThanh", "DaHuy" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction("OrderDetail", new { id });
            }

            order.Status = status;
            order.UpdatedAt = DateTime.Now;

            // If cancelled, restore stock
            if (status == "DaHuy")
            {
                var orderItems = await _context.OrderItems
                    .Where(oi => oi.OrderId == id)
                    .ToListAsync();

                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQty += item.Quantity;
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng";
            return RedirectToAction("OrderDetail", new { id });
        }

        // Product Management
        [HttpGet("/admin/san-pham")]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

        [HttpGet("/admin/san-pham/them")]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View();
        }

        [HttpPost("/admin/san-pham/them")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(string title, int authorId, string? isbn, 
            decimal price, int stockQty, List<int>? categoryIds, string status, string? description, 
            IFormFile? coverImage, string? publisher)
        {
            var product = new Product
            {
                Title = title,
                AuthorId = authorId,
                ISBN = isbn ?? string.Empty,
                Price = price,
                StockQty = stockQty,
                Status = status,
                Description = description ?? string.Empty,
                Publisher = publisher ?? string.Empty,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Handle cover image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(fileStream);
                }
                
                product.CoverImageUrl = "/images/products/" + uniqueFileName;
            }
            else
            {
                product.CoverImageUrl = "/images/products/default.jpg";
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Add categories
            if (categoryIds != null && categoryIds.Any())
            {
                foreach (var categoryId in categoryIds)
                {
                    _context.ProductCategories.Add(new ProductCategory
                    {
                        ProductId = product.ProductId,
                        CategoryId = categoryId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Đã thêm sản phẩm thành công";
            return RedirectToAction("Products");
        }

        [HttpGet("/admin/san-pham/sua/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Authors = await _context.Authors.OrderBy(a => a.Name).ToListAsync();
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.SelectedCategories = product.ProductCategories.Select(pc => pc.CategoryId).ToList();

            return View(product);
        }

        [HttpPost("/admin/san-pham/cap-nhat")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int productId, string title, int authorId, string? isbn, 
            decimal price, int stockQty, List<int>? categoryIds, string status, string? description, 
            IFormFile? coverImage, string? publisher)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound();
            }

            product.Title = title;
            product.AuthorId = authorId;
            product.ISBN = isbn ?? string.Empty;
            product.Price = price;
            product.StockQty = stockQty;
            product.Status = status;
            product.Description = description ?? string.Empty;
            product.Publisher = publisher ?? string.Empty;
            product.UpdatedAt = DateTime.Now;

            // Handle cover image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(fileStream);
                }
                
                product.CoverImageUrl = "/images/products/" + uniqueFileName;
            }

            // Update categories
            _context.ProductCategories.RemoveRange(product.ProductCategories);
            if (categoryIds != null && categoryIds.Any())
            {
                foreach (var categoryId in categoryIds)
                {
                    _context.ProductCategories.Add(new ProductCategory
                    {
                        ProductId = productId,
                        CategoryId = categoryId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật sản phẩm thành công";
            return RedirectToAction("Products");
        }

        [HttpPost("/admin/san-pham/xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Soft delete by changing status
            product.Status = "Discontinued";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa sản phẩm thành công";
            return RedirectToAction("Products");
        }

        // Category Management
        [HttpGet("/admin/danh-muc")]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.ProductCategories)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        [HttpPost("/admin/danh-muc/them")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, string slug, string description, IFormFile? backgroundImage)
        {
            var category = new Category
            {
                Name = name,
                Slug = slug,
                Description = description ?? string.Empty
            };

            if (backgroundImage != null && backgroundImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(backgroundImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundImage.CopyToAsync(fileStream);
                }
                
                category.BackgroundImageUrl = "/images/categories/" + uniqueFileName;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm danh mục thành công";
            return RedirectToAction("Categories");
        }

        [HttpPost("/admin/danh-muc/sua")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int categoryId, string name, string slug, string description, IFormFile? backgroundImage)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = name;
            category.Slug = slug;
            category.Description = description ?? string.Empty;

            if (backgroundImage != null && backgroundImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                Directory.CreateDirectory(uploadsFolder);
                
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(backgroundImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await backgroundImage.CopyToAsync(fileStream);
                }
                
                category.BackgroundImageUrl = "/images/categories/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật danh mục thành công";
            return RedirectToAction("Categories");
        }

        [HttpPost("/admin/danh-muc/xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ProductCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
                
            if (category == null)
            {
                return NotFound();
            }

            // Remove product-category relationships
            _context.ProductCategories.RemoveRange(category.ProductCategories);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa danh mục thành công";
            return RedirectToAction("Categories");
        }

        // Customer Management
        [HttpGet("/admin/khach-hang")]
        public async Task<IActionResult> Customers()
        {
            var customers = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            // Get order statistics for each customer
            var customerOrders = new Dictionary<string, DOAN_Web.ViewModels.CustomerOrderStats>();
            foreach (var customer in customers)
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == customer.Id && o.Status == "HoanThanh")
                    .ToListAsync();
                
                customerOrders[customer.Id] = new DOAN_Web.ViewModels.CustomerOrderStats
                {
                    Count = orders.Count,
                    Sum = orders.Sum(o => o.Total)
                };
            }
            
            ViewBag.CustomerOrders = customerOrders;

            return View(customers);
        }

        [HttpGet("/admin/khach-hang/{id}")]
        public async Task<IActionResult> CustomerDetail(string id)
        {
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            ViewBag.Orders = orders;

            return View(customer);
        }

        // Author Management
        [HttpGet("/admin/tac-gia")]
        public async Task<IActionResult> Authors()
        {
            var authors = await _context.Authors
                .Include(a => a.Products)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(authors);
        }

        [HttpPost("/admin/tac-gia/them")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAuthor(string name, string? bio)
        {
            var author = new Author
            {
                Name = name,
                Bio = bio ?? string.Empty
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm tác giả thành công";
            return RedirectToAction("Authors");
        }

        [HttpPost("/admin/tac-gia/sua")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuthor(int authorId, string name, string? bio)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            author.Name = name;
            author.Bio = bio ?? string.Empty;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã cập nhật tác giả thành công";
            return RedirectToAction("Authors");
        }

        [HttpPost("/admin/tac-gia/xoa/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Products)
                .FirstOrDefaultAsync(a => a.AuthorId == id);
                
            if (author == null)
            {
                return NotFound();
            }

            if (author.Products.Any())
            {
                TempData["Error"] = "Không thể xóa tác giả đang có sản phẩm";
                return RedirectToAction("Authors");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa tác giả thành công";
            return RedirectToAction("Authors");
        }
    }
}
