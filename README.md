# Shopjoy - Vietnamese Book E-commerce Store

## Project Overview
Shopjoy is a full-featured Vietnamese book e-commerce website built with ASP.NET Core 9.0 MVC, Entity Framework Core, and SQL Server LocalDB. The project features a beautiful UI with Bootstrap 5, GSAP, and Anime.js animations.

## Tech Stack
- **Backend:** ASP.NET Core 9.0 MVC
- **Database:** SQL Server LocalDB
- **ORM:** Entity Framework Core 9.0
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5.3, jQuery, GSAP, Anime.js
- **Language:** Vietnamese UI

## Features
### Customer Features
- Browse books by category
- Search and filter products
- Product detail pages with reviews
- Shopping cart functionality
- Secure checkout process
- Manual payment via bank transfer with QR code
- Upload payment proof
- Order history and tracking

### Admin Features
- Dashboard with statistics
- Order management
- Payment verification
- Order status updates
- Product management (CRUD operations)

## Database Schema
The application uses the following main entities:
- **ApplicationUser** - Extended IdentityUser with custom fields
- **Product** - Book products with details
- **Category** - Product categories
- **Author** - Book authors
- **Order** - Customer orders
- **OrderItem** - Order line items
- **CartItem** - Shopping cart items
- **PaymentProof** - Payment verification images
- **Review** - Product reviews
- **Address** - Customer shipping addresses

## Setup Instructions

### Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DOAN_Web
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update connection string (if needed)**
   Edit `appsettings.json` to modify the database connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShopjoyDB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

## Payment Flow
1. Customer adds products to cart
2. Proceeds to checkout and fills shipping information
3. Places order
4. System displays payment instructions with bank details and QR code
5. Customer transfers money and uploads payment proof
6. Admin verifies payment proof
7. Admin updates order status (Confirmed → Shipping → Delivered)

## Project Structure
```
DOAN_Web/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── ProductController.cs
│   ├── CartController.cs
│   ├── CheckoutController.cs
│   ├── CatalogController.cs
│   └── AdminController.cs
├── Models/              # Domain models
│   ├── ApplicationUser.cs
│   ├── Product.cs
│   ├── Category.cs
│   ├── Order.cs
│   └── ...
├── ViewModels/          # View models
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── CartViewModel.cs
│   └── CheckoutViewModel.cs
├── Views/               # Razor views
│   ├── Home/
│   ├── Account/
│   ├── Product/
│   ├── Cart/
│   ├── Checkout/
│   ├── Catalog/
│   ├── Admin/
│   └── Shared/
├── Data/                # Data access layer
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
└── wwwroot/             # Static files
    ├── css/
    ├── js/
    └── images/
```

## Vietnamese Text Examples

### UI Labels
- Trang chủ (Home)
- Đăng nhập (Login)
- Đăng ký (Register)
- Giỏ hàng (Cart)
- Thanh toán (Checkout)
- Tìm kiếm (Search)
- Danh mục (Categories)

### Order Status
- Chờ thanh toán (Pending Payment)
- Đã xác nhận (Payment Verified)
- Đang giao (Shipping)
- Hoàn thành (Delivered)
- Đã hủy (Cancelled)

## API Endpoints

### Cart Management
- `POST /cart/them` - Add to cart
- `POST /cart/cap-nhat` - Update quantity
- `POST /cart/xoa/{id}` - Remove item
- `GET /cart/count` - Get cart count

### Checkout
- `GET /thanh-toan` - Checkout page
- `POST /thanh-toan/dat-hang` - Place order
- `GET /don-hang/{id}/thanh-toan` - Payment instructions
- `POST /don-hang/{id}/xac-nhan-thanh-toan` - Upload payment proof

### Admin
- `GET /admin` - Dashboard
- `GET /admin/don-hang` - Order list
- `GET /admin/don-hang/{id}` - Order detail
- `POST /admin/don-hang/{id}/xac-nhan-thanh-toan` - Verify payment
- `POST /admin/don-hang/{id}/cap-nhat-trang-thai` - Update order status

## Animations
- **GSAP:** Product grid fade-in, scroll animations
- **Anime.js:** Button hover effects, cart icon animations
- **Spline:** 3D hero section on homepage

## Browser Support
- Chrome (latest)
- Firefox (latest)
- Edge (latest)
- Safari (latest)

## Known Issues & Future Enhancements
- Payment QR code image needs to be added to `/wwwroot/images/payment-qr.png`
- Real-time notifications for admin on new orders
- Advanced product filtering
- Product reviews and ratings system
- Wishlist functionality
- Email notifications

