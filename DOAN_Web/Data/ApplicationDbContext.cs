using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DOAN_Web.Models;

namespace DOAN_Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PaymentProof> PaymentProofs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for all decimal properties
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(o => o.ShippingFee).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });

            // ProductCategory already has ProductCategoryId as primary key in the model
            // No need for composite key configuration

            // Configure Review relationships to prevent cascade delete conflicts
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure OrderItem relationships to prevent cascade delete conflicts
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure PaymentProof relationships
            modelBuilder.Entity<PaymentProof>()
                .HasOne(pp => pp.Order)
                .WithOne(o => o.PaymentProof)
                .HasForeignKey<PaymentProof>(pp => pp.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
