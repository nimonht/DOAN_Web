using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Models;

namespace DOAN_Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync(roleManager);

            // Seed admin user
            await SeedAdminAsync(userManager);

            // Seed data
            await SeedAuthorsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@bookstore.com";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedAuthorsAsync(ApplicationDbContext context)
        {
            if (await context.Authors.AnyAsync())
                return;

            var authors = new List<Author>
            {
                new Author
                {
                    Name = "J.K. Rowling",
                    Bio = "British author, best known for the Harry Potter series.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "George R.R. Martin",
                    Bio = "American novelist and short story writer, known for A Song of Ice and Fire.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "J.R.R. Tolkien",
                    Bio = "English writer and philologist, author of The Hobbit and The Lord of the Rings.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "Stephen King",
                    Bio = "American author of horror, supernatural fiction, suspense, and fantasy novels.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "Agatha Christie",
                    Bio = "English writer known for her detective novels featuring Hercule Poirot and Miss Marple.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "Dan Brown",
                    Bio = "American author best known for his thriller novels, including The Da Vinci Code.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "Harper Lee",
                    Bio = "American novelist best known for To Kill a Mockingbird.",
                    CreatedAt = DateTime.Now
                },
                new Author
                {
                    Name = "Jane Austen",
                    Bio = "English novelist known for her romantic fiction set among the landed gentry.",
                    CreatedAt = DateTime.Now
                }
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Fantasy",
                    Slug = "fantasy",
                    Description = "Magical worlds, mythical creatures, and epic adventures.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    Name = "Mystery & Thriller",
                    Slug = "mystery-thriller",
                    Description = "Suspenseful tales that keep you on the edge of your seat.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    Name = "Horror",
                    Slug = "horror",
                    Description = "Frightening stories designed to scare and unsettle.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1509266272358-7701da638078?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    Name = "Romance",
                    Slug = "romance",
                    Description = "Love stories and romantic adventures.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1474552226712-ac0f0961a954?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    Name = "Classic Literature",
                    Slug = "classic-literature",
                    Description = "Timeless works of literary significance.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1457369804613-52c61a468e7d?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                    Name = "Science Fiction",
                    Slug = "science-fiction",
                    Description = "Futuristic and scientific imagination.",
                    BackgroundImageUrl = "https://images.unsplash.com/photo-1446776653964-20c1d3a81b06?w=800&h=400&fit=crop",
                    CreatedAt = DateTime.Now
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
                return;

            var authors = await context.Authors.ToListAsync();
            var categories = await context.Categories.ToListAsync();

            var products = new List<Product>
            {
                new Product
                {
                    Title = "Harry Potter and the Philosopher's Stone",
                    Slug = "harry-potter-philosophers-stone",
                    ISBN = "9780747532699",
                    AuthorId = authors.First(a => a.Name == "J.K. Rowling").AuthorId,
                    Publisher = "Bloomsbury Publishing",
                    PublicationDate = new DateTime(1997, 6, 26),
                    Description = "The first novel in the Harry Potter series follows Harry Potter, a young wizard who discovers his magical heritage on his eleventh birthday.",
                    Price = 299000,
                    StockQty = 50,
                    CoverImageUrl = "https://images.unsplash.com/photo-1621351183012-e2f9972dd9bf?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "A Game of Thrones",
                    Slug = "game-of-thrones",
                    ISBN = "9780553103540",
                    AuthorId = authors.First(a => a.Name == "George R.R. Martin").AuthorId,
                    Publisher = "Bantam Spectra",
                    PublicationDate = new DateTime(1996, 8, 1),
                    Description = "The first book in A Song of Ice and Fire series, set in the fictional Seven Kingdoms of Westeros.",
                    Price = 350000,
                    StockQty = 35,
                    CoverImageUrl = "https://images.unsplash.com/photo-1614544048536-0d28caf77f41?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "The Lord of the Rings",
                    Slug = "lord-of-the-rings",
                    ISBN = "9780544003415",
                    AuthorId = authors.First(a => a.Name == "J.R.R. Tolkien").AuthorId,
                    Publisher = "Houghton Mifflin Harcourt",
                    PublicationDate = new DateTime(1954, 7, 29),
                    Description = "Epic high-fantasy novel about the quest to destroy the One Ring and defeat the Dark Lord Sauron.",
                    Price = 450000,
                    StockQty = 40,
                    CoverImageUrl = "https://images.unsplash.com/photo-1618932260643-eee4a2f652a6?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "The Shining",
                    Slug = "the-shining",
                    ISBN = "9780307743657",
                    AuthorId = authors.First(a => a.Name == "Stephen King").AuthorId,
                    Publisher = "Doubleday",
                    PublicationDate = new DateTime(1977, 1, 28),
                    Description = "A family's winter caretaking job at an isolated hotel descends into madness.",
                    Price = 280000,
                    StockQty = 45,
                    CoverImageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "Murder on the Orient Express",
                    Slug = "murder-orient-express",
                    ISBN = "9780062693662",
                    AuthorId = authors.First(a => a.Name == "Agatha Christie").AuthorId,
                    Publisher = "William Collins",
                    PublicationDate = new DateTime(1934, 1, 1),
                    Description = "Detective Hercule Poirot investigates a murder on the famous train.",
                    Price = 220000,
                    StockQty = 60,
                    CoverImageUrl = "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "The Da Vinci Code",
                    Slug = "da-vinci-code",
                    ISBN = "9780307474278",
                    AuthorId = authors.First(a => a.Name == "Dan Brown").AuthorId,
                    Publisher = "Doubleday",
                    PublicationDate = new DateTime(2003, 3, 18),
                    Description = "A mystery thriller following symbologist Robert Langdon as he investigates a murder in the Louvre.",
                    Price = 320000,
                    StockQty = 55,
                    CoverImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "To Kill a Mockingbird",
                    Slug = "to-kill-mockingbird",
                    ISBN = "9780061120084",
                    AuthorId = authors.First(a => a.Name == "Harper Lee").AuthorId,
                    Publisher = "J. B. Lippincott & Co.",
                    PublicationDate = new DateTime(1960, 7, 11),
                    Description = "A classic of modern American literature exploring themes of racial injustice and childhood innocence.",
                    Price = 250000,
                    StockQty = 70,
                    CoverImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "Pride and Prejudice",
                    Slug = "pride-and-prejudice",
                    ISBN = "9780141439518",
                    AuthorId = authors.First(a => a.Name == "Jane Austen").AuthorId,
                    Publisher = "Penguin Classics",
                    PublicationDate = new DateTime(1813, 1, 28),
                    Description = "A romantic novel of manners exploring the issues of marriage, morality, and misconceptions.",
                    Price = 180000,
                    StockQty = 65,
                    CoverImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "It",
                    Slug = "it-stephen-king",
                    ISBN = "9781501142970",
                    AuthorId = authors.First(a => a.Name == "Stephen King").AuthorId,
                    Publisher = "Viking Press",
                    PublicationDate = new DateTime(1986, 9, 15),
                    Description = "A group of childhood friends confront an evil entity that appears as a clown.",
                    Price = 380000,
                    StockQty = 30,
                    CoverImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                },
                new Product
                {
                    Title = "The Hobbit",
                    Slug = "the-hobbit",
                    ISBN = "9780547928227",
                    AuthorId = authors.First(a => a.Name == "J.R.R. Tolkien").AuthorId,
                    Publisher = "Houghton Mifflin Harcourt",
                    PublicationDate = new DateTime(1937, 9, 21),
                    Description = "Bilbo Baggins' unexpected journey to help a group of dwarves reclaim their mountain home.",
                    Price = 320000,
                    StockQty = 55,
                    CoverImageUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?w=400&h=600&fit=crop",
                    Status = "Active",
                    CreatedAt = DateTime.Now
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // Create product-category relationships
            var productCategories = new List<ProductCategory>
            {
                // Harry Potter - Fantasy
                new ProductCategory { ProductId = products[0].ProductId, CategoryId = categories.First(c => c.Slug == "fantasy").CategoryId },
                
                // Game of Thrones - Fantasy
                new ProductCategory { ProductId = products[1].ProductId, CategoryId = categories.First(c => c.Slug == "fantasy").CategoryId },
                
                // Lord of the Rings - Fantasy
                new ProductCategory { ProductId = products[2].ProductId, CategoryId = categories.First(c => c.Slug == "fantasy").CategoryId },
                
                // The Shining - Horror
                new ProductCategory { ProductId = products[3].ProductId, CategoryId = categories.First(c => c.Slug == "horror").CategoryId },
                
                // Murder on Orient Express - Mystery
                new ProductCategory { ProductId = products[4].ProductId, CategoryId = categories.First(c => c.Slug == "mystery-thriller").CategoryId },
                
                // Da Vinci Code - Mystery
                new ProductCategory { ProductId = products[5].ProductId, CategoryId = categories.First(c => c.Slug == "mystery-thriller").CategoryId },
                
                // To Kill a Mockingbird - Classic
                new ProductCategory { ProductId = products[6].ProductId, CategoryId = categories.First(c => c.Slug == "classic-literature").CategoryId },
                
                // Pride and Prejudice - Classic & Romance
                new ProductCategory { ProductId = products[7].ProductId, CategoryId = categories.First(c => c.Slug == "classic-literature").CategoryId },
                new ProductCategory { ProductId = products[7].ProductId, CategoryId = categories.First(c => c.Slug == "romance").CategoryId },
                
                // It - Horror
                new ProductCategory { ProductId = products[8].ProductId, CategoryId = categories.First(c => c.Slug == "horror").CategoryId },
                
                // The Hobbit - Fantasy
                new ProductCategory { ProductId = products[9].ProductId, CategoryId = categories.First(c => c.Slug == "fantasy").CategoryId }
            };

            await context.ProductCategories.AddRangeAsync(productCategories);
            await context.SaveChangesAsync();
        }
    }
}
