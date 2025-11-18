# BÁO CÁO PHÂN TÍCH ỨNG DỤNG WEB BÁN SÁCH TRỰC TUYẾN

## TỔNG QUAN ỨNG DỤNG

**Tên dự án:** DOAN_Web - Hệ thống bán sách trực tuyến Shopjoy

**Công nghệ sử dụng:** ASP.NET Core MVC, Entity Framework Core, Identity, Bootstrap 5.3

**Mô hình kiến trúc:** MVC (Model-View-Controller)

**Cơ sở dữ liệu:** SQL Server (thông qua Entity Framework Core)

---

## 6. PHÂN TÍCH LAYOUT VÀ CHỨC NĂNG

### 6.1. PHẦN ADMIN

#### 6.1.1. Trang LAYOUT (_AdminLayout.cshtml)

**Cấu trúc layout:**

| Thành phần | Mô tả |
|------------|-------|
| **Logo & Header** | - Logo "Shopjoy Admin" với icon sách<br>- Thanh navigation phía trên<br>- Nút "Về trang chủ"<br>- Menu người dùng (dropdown) |
| **Sidebar** | - Dashboard<br>- Quản lý đơn hàng<br>- Quản lý sản phẩm<br>- Quản lý danh mục<br>- Quản lý tác giả<br>- Quản lý khách hàng<br>- Responsive cho mobile với nút toggle |
| **Content Body** | Khu vực hiển thị nội dung chính của từng trang quản trị |
| **Footer** | Footer cố định ở cuối trang |

**Đặc điểm:**
- Layout responsive với Bootstrap 5.3
- Sidebar cố định bên trái (desktop), có thể toggle (mobile)
- Thanh navigation cố định ở trên
- Chỉ admin mới truy cập được (Authorization: Roles = "Admin")

#### 6.1.2. Quản lý Dashboard

**Chức năng chính:**
- Hiển thị thống kê tổng quan:
  - Tổng số đơn hàng
  - Đơn hàng chờ thanh toán
  - Tổng doanh thu (từ đơn hàng hoàn thành)
  - Tổng số sản phẩm
  - Số sản phẩm đang bán
  
- Biểu đồ doanh thu theo tháng (12 tháng)
- Biểu đồ phân bổ trạng thái đơn hàng (Chờ thanh toán, Đã xác nhận, Đang giao, Hoàn thành, Đã hủy)
- Top 5 sản phẩm bán chạy nhất
- 10 đơn hàng gần nhất

**Controller:** AdminController  
**Action Method:** Dashboard  
**View:** Dashboard.cshtml

#### 6.1.3. Quản lý Đơn hàng

**Chức năng:**
- Xem danh sách tất cả đơn hàng
- Lọc đơn hàng theo trạng thái:
  - ChoThanhToan
  - DaXacNhan
  - DangGiao
  - HoanThanh
  - DaHuy
  
- Xem chi tiết đơn hàng
- Xác nhận thanh toán (khi khách hàng upload chứng từ)
- Cập nhật trạng thái đơn hàng
- Tự động hoàn trả số lượng khi hủy đơn

**Controller:** AdminController  
**Action Methods:**
- Orders (GET) - Danh sách đơn hàng
- OrderDetail (GET) - Chi tiết đơn hàng
- VerifyPayment (POST) - Xác nhận thanh toán
- UpdateOrderStatus (POST) - Cập nhật trạng thái

**Views:**
- Orders.cshtml - Danh sách đơn hàng
- OrderDetail.cshtml - Chi tiết đơn hàng

#### 6.1.4. Quản lý Sản phẩm

**Chức năng:**
- Xem danh sách sản phẩm (kèm tác giả, danh mục)
- Thêm sản phẩm mới:
  - Tiêu đề, ISBN, tác giả, nhà xuất bản
  - Giá, số lượng tồn kho
  - Mô tả, trạng thái
  - Upload ảnh bìa
  - Chọn danh mục (nhiều danh mục)
  
- Sửa sản phẩm
- Xóa sản phẩm (soft delete - chuyển trạng thái sang "Discontinued")

**Controller:** AdminController  
**Action Methods:**
- Products (GET) - Danh sách sản phẩm
- AddProduct (GET) - Form thêm sản phẩm
- CreateProduct (POST) - Xử lý thêm sản phẩm
- EditProduct (GET) - Form sửa sản phẩm
- UpdateProduct (POST) - Xử lý cập nhật
- DeleteProduct (POST) - Xóa sản phẩm

**Views:**
- Products.cshtml - Danh sách
- AddProduct.cshtml - Form thêm
- EditProduct.cshtml - Form sửa

#### 6.1.5. Quản lý Danh mục

**Chức năng:**
- Xem danh sách danh mục
- Thêm danh mục mới (tên, slug, mô tả, ảnh nền)
- Sửa danh mục
- Xóa danh mục (hard delete, nhưng phải xóa các mối quan hệ product-category trước)

**Controller:** AdminController  
**Action Methods:**
- Categories (GET)
- AddCategory (POST)
- EditCategory (POST)
- DeleteCategory (POST)

**View:** Categories.cshtml

#### 6.1.6. Quản lý Tác giả

**Chức năng:**
- Xem danh sách tác giả (kèm số lượng sản phẩm)
- Thêm tác giả mới (tên, tiểu sử)
- Sửa tác giả
- Xóa tác giả (không được có sản phẩm)

**Controller:** AdminController  
**Action Methods:**
- Authors (GET)
- AddAuthor (POST)
- EditAuthor (POST)
- DeleteAuthor (POST)

**View:** Authors.cshtml

#### 6.1.7. Quản lý Khách hàng

**Chức năng:**
- Xem danh sách khách hàng
- Thống kê số đơn hàng và tổng chi tiêu của mỗi khách hàng
- Xem chi tiết khách hàng và lịch sử đơn hàng

**Controller:** AdminController  
**Action Methods:**
- Customers (GET) - Danh sách khách hàng
- CustomerDetail (GET) - Chi tiết khách hàng

**Views:**
- Customers.cshtml
- CustomerDetail.cshtml

---

### 6.2. PHẦN KHÁCH HÀNG

#### 6.2.1. Trang MASTER LAYOUT (_Layout.cshtml)

##### 6.2.1.1. Phân tích layout

| Thành phần | Mô tả |
|------------|-------|
| **Logo** | Logo "Shopjoy" với icon sách, link về trang chủ |
| **Thanh Menu** | - Trang chủ<br>- Danh mục (dropdown)<br>- Sách mới |
| **Thanh Search** | Form tìm kiếm sách (input + button) |
| **Giỏ hàng** | - Icon giỏ hàng<br>- Badge hiển thị số lượng sản phẩm<br>- Link đến trang giỏ hàng |
| **Đăng nhập/Tài khoản** | - Nếu chưa đăng nhập: nút Đăng nhập/Đăng ký<br>- Nếu đã đăng nhập: dropdown menu với:<br>  + Tài khoản<br>  + Đơn hàng của tôi<br>  + Quản trị (nếu là Admin)<br>  + Đăng xuất |
| **Content Body** | Khu vực hiển thị nội dung động |
| **Footer** | Thông tin công ty, liên hệ, mạng xã hội |

##### 6.2.1.2. Phân tích chức năng

| View | Partial View | Model | Mô tả |
|------|--------------|-------|-------|
| _Layout.cshtml | (Form tìm kiếm) | N/A | Layout chính cho khách hàng |
| | (Cart count badge) | CartItem | Hiển thị số lượng sản phẩm trong giỏ |
| | (User menu) | ApplicationUser | Menu người dùng |

**JavaScript chức năng:**
- Cập nhật số lượng giỏ hàng động (AJAX)
- Toggle menu danh mục
- Sticky header khi scroll

#### 6.2.2. Trang HOME (Index.cshtml)

##### 6.2.2.1. Phân tích layout

| Vị trí | Thành phần | Mô tả |
|--------|-----------|-------|
| **Hero Section** | Banner 3D (Spline) | - Hiệu ứng 3D particles hand<br>- Tiêu đề "Kết nối tri thức"<br>- Nút "Mua sắm ngay" |
| **Section 1** | Sách nổi bật | - 12 sản phẩm mới nhất<br>- Hiển thị grid 4 cột (desktop)<br>- Ảnh bìa, tên, tác giả, giá<br>- Nút thêm vào giỏ hàng |
| **Section 2** | Danh mục sách | - Danh sách các danh mục<br>- Ảnh nền cho mỗi danh mục<br>- Link đến trang danh mục |

##### 6.2.2.2. Phân tích chức năng

| Controller | View | Model | Mô tả |
|------------|------|-------|-------|
| HomeController | Index.cshtml | Product, Author | - Lấy 12 sản phẩm mới nhất (Active)<br>- Sắp xếp theo CreatedAt giảm dần |
| | | Category | Lấy tất cả danh mục (kèm ProductCategories) |

**Chức năng JavaScript:**
- Thêm vào giỏ hàng (AJAX)
- Hiển thị thông báo toast
- Cập nhật số lượng giỏ hàng

#### 6.2.3. Trang DANH SÁCH SẢN PHẨM

**URL:** `/danh-muc/{slug}` hoặc `/tim-kiem`

##### Trang Danh mục (Category)

**Chức năng:**
- Hiển thị sản phẩm theo danh mục
- Phân trang (12 sản phẩm/trang)
- Chỉ hiển thị sản phẩm Active

**Controller:** CatalogController  
**Action Method:** Category(string slug, int page)  
**View:** Category.cshtml  
**Model:** Product, Category

##### Trang Tìm kiếm (Search)

**Chức năng:**
- Tìm kiếm theo từ khóa (title, description, author name)
- Lọc theo khoảng giá (min, max)
- Lọc theo danh mục
- Sắp xếp theo:
  - Giá tăng/giảm dần
  - Tên A-Z/Z-A
  - Mới nhất (mặc định)
- Phân trang (12 sản phẩm/trang)

**Controller:** ProductController  
**Action Method:** Search(string q, int? minPrice, int? maxPrice, int? categoryId, string sortBy, int page)  
**View:** Search.cshtml  
**Model:** Product, Author, Category

#### 6.2.4. Trang CHI TIẾT SẢN PHẨM

**URL:** `/sach/{slug}`

**Chức năng:**
- Hiển thị thông tin chi tiết sản phẩm:
  - Ảnh bìa
  - Tiêu đề, tác giả, nhà xuất bản
  - ISBN, giá, số lượng tồn kho
  - Mô tả chi tiết
  - Danh mục
  
- Gallery ảnh sản phẩm (nếu có)
- Đánh giá và xếp hạng:
  - Hiển thị điểm trung bình
  - Danh sách đánh giá (sắp xếp theo ngày)
  - Form đánh giá (chỉ cho khách đã mua và chưa đánh giá)
  
- Sản phẩm liên quan (cùng danh mục, tối đa 6)
- Nút thêm vào giỏ hàng
- Chọn số lượng

**Controller:** ProductController  
**Action Methods:**
- Detail(string slug) - GET
- AddReview(ReviewInputModel input) - POST

**View:** Detail.cshtml  
**Model:** Product, Author, Category, Review, ApplicationUser

**Business Logic:**
- Khách hàng chỉ có thể đánh giá khi:
  - Đã đăng nhập
  - Đã mua sản phẩm (đơn hàng hoàn thành)
  - Chưa đánh giá trước đó

#### 6.2.5. Trang GIỎ HÀNG

**URL:** `/gio-hang`

**Chức năng:**
- Hiển thị danh sách sản phẩm trong giỏ
- Cập nhật số lượng (AJAX)
- Xóa sản phẩm khỏi giỏ
- Hiển thị tổng tiền
- Nút "Tiến hành thanh toán"

**Lưu trữ giỏ hàng:**
- Khách đã đăng nhập: lưu trong database (bảng CartItem)
- Khách chưa đăng nhập: lưu trong Session

**Controller:** CartController  
**Action Methods:**
- Index (GET) - Xem giỏ hàng
- AddToCart (POST) - Thêm vào giỏ
- UpdateQuantity (POST) - Cập nhật số lượng
- RemoveFromCart (POST) - Xóa khỏi giỏ
- GetCartCount (GET) - Lấy số lượng (cho badge)

**View:** Index.cshtml  
**Model:** CartViewModel, CartItemViewModel

#### 6.2.6. Trang THANH TOÁN

**URL:** `/thanh-toan`

**Chức năng:**
- Form nhập thông tin giao hàng:
  - Họ tên người nhận
  - Số điện thoại
  - Địa chỉ chi tiết (Địa chỉ, Phường/Xã, Quận/Huyện, Tỉnh/Thành phố)
  - Ghi chú
  
- Hiển thị tóm tắt đơn hàng:
  - Danh sách sản phẩm
  - Tạm tính
  - Phí vận chuyển (30,000đ cố định)
  - Tổng cộng
  
- Nút "Đặt hàng"

**Business Logic:**
- Kiểm tra đăng nhập (bắt buộc)
- Kiểm tra giỏ hàng không rỗng
- Kiểm tra số lượng tồn kho
- Tạo đơn hàng với mã tự động (ORDyyyyMMddnnn)
- Tạo các OrderItem
- Trừ số lượng tồn kho
- Xóa giỏ hàng sau khi đặt hàng
- Chuyển đến trang hướng dẫn thanh toán

**Controller:** CheckoutController  
**Action Methods:**
- Index (GET) - Form thanh toán
- PlaceOrder (POST) - Xử lý đặt hàng

**View:** Index.cshtml  
**Model:** CheckoutViewModel, CartViewModel

#### 6.2.7. Trang XÁC NHẬN ĐƠN HÀNG / HƯỚNG DẪN THANH TOÁN

**URL:** `/don-hang/{id}/thanh-toan`

**Chức năng:**
- Hiển thị thông tin đơn hàng vừa đặt
- Hiển thị mã đơn hàng
- Hiển thị tổng tiền cần thanh toán
- Hướng dẫn chuyển khoản:
  - Ngân hàng
  - Số tài khoản
  - Tên người nhận
  - Nội dung chuyển khoản (mã đơn hàng)
  
- Form upload ảnh chứng từ chuyển khoản
- Nút "Xác nhận đã chuyển khoản"

**Business Logic:**
- Chỉ chủ đơn hàng mới xem được
- Upload ảnh (JPG, JPEG, PNG, tối đa 5MB)
- Lưu vào bảng PaymentProof
- Chuyển về trang "Đơn hàng của tôi" sau khi upload

**Controller:** CheckoutController  
**Action Methods:**
- PaymentInstructions (GET)
- UploadPaymentProof (POST)

**View:** PaymentInstructions.cshtml  
**Model:** Order, OrderItem, Product

#### 6.2.8. Trang LỊCH SỬ MUA HÀNG

**URL:** `/tai-khoan/don-hang-cua-toi`

**Chức năng:**
- Hiển thị danh sách đơn hàng của khách (mới nhất trước)
- Thông tin mỗi đơn:
  - Mã đơn hàng
  - Ngày đặt
  - Trạng thái
  - Tổng tiền
  - Danh sách sản phẩm
  
- Badge trạng thái với màu sắc:
  - ChoThanhToan: warning (vàng)
  - DaXacNhan: info (xanh dương)
  - DangGiao: primary (xanh dương đậm)
  - HoanThanh: success (xanh lá)
  - DaHuy: danger (đỏ)
  
- Link đến trang thanh toán (nếu chưa thanh toán)
- Hiển thị form đánh giá cho sản phẩm đã mua và hoàn thành (nếu chưa đánh giá)

**Controller:** AccountController  
**Action Method:** DonHangCuaToi  
**View:** DonHangCuaToi.cshtml  
**Model:** Order, OrderItem, Product, Review

#### 6.2.9. ĐĂNG KÝ / ĐĂNG NHẬP

##### Trang Đăng nhập

**URL:** `/tai-khoan/dang-nhap`

**Chức năng:**
- Form đăng nhập:
  - Email
  - Mật khẩu
  - Ghi nhớ đăng nhập (checkbox)
- Link "Quên mật khẩu"
- Link "Đăng ký tài khoản mới"
- Redirect về trang trước đó sau khi đăng nhập (returnUrl)

**Controller:** AccountController  
**Action Methods:**
- DangNhap (GET)
- DangNhap (POST)

**View:** DangNhap.cshtml  
**Model:** LoginViewModel

##### Trang Đăng ký

**URL:** `/tai-khoan/dang-ky`

**Chức năng:**
- Form đăng ký:
  - Họ và tên
  - Email
  - Số điện thoại
  - Mật khẩu
  - Xác nhận mật khẩu
  
- Validation:
  - Email hợp lệ
  - Số điện thoại hợp lệ
  - Mật khẩu tối thiểu 6 ký tự
  - Mật khẩu khớp
  
- Tự động đăng nhập sau khi đăng ký thành công
- Tự động gán role "Customer"

**Controller:** AccountController  
**Action Methods:**
- DangKy (GET)
- DangKy (POST)

**View:** DangKy.cshtml  
**Model:** RegisterViewModel

##### Trang Tài khoản

**URL:** `/tai-khoan/thong-tin`

**Chức năng:**
- Hiển thị thông tin cá nhân:
  - Họ và tên
  - Email
  - Số điện thoại
  - Ngày tạo tài khoản

**Controller:** AccountController  
**Action Method:** TaiKhoan  
**View:** TaiKhoan.cshtml  
**Model:** ApplicationUser

---
## 7. TỔNG KẾT CHỨC NĂNG THEO VAI TRÒ SỬ DỤNG

| Action Method / Model | Create | Edit | Delete | Index | Detail |
|----------------------|--------|------|--------|-------|--------|
| **Category** | Admin | Admin | Admin | Admin (all)<br>Customer (menu, catalog) | Customer (catalog) |
| **Product** | Admin | Admin | Admin (soft delete) | Admin (all)<br>Customer (home, catalog, search) | Customer (detail page) |
| **Order &<br>Order Detail** | Customer (đặt hàng) | Admin (cập nhật trạng thái)<br>Customer (upload chứng từ) | N/A | Admin (all)<br>Customer (lịch sử cá nhân) | Admin (all)<br>Customer (đơn hàng cá nhân) |
| **Customer<br>(ApplicationUser)** | Customer (đăng ký) | Customer (thông tin cá nhân) | N/A | Admin | Admin (all)<br>Customer (thông tin cá nhân) |
| **Author** | Admin | Admin | Admin | Admin | N/A |
| **Review** | Customer (đã mua & hoàn thành) | N/A | N/A | Customer (trong detail page) | N/A |
| **CartItem** | Customer (thêm vào giỏ) | Customer (cập nhật số lượng) | Customer (xóa khỏi giỏ) | Customer (giỏ hàng) | N/A |
| **PaymentProof** | Customer (upload) | N/A | N/A | Admin (trong order detail) | Admin (xác nhận thanh toán) |

---

## 8. MÔ TẢ CHI TIẾT ỨNG DỤNG THEO MÔ HÌNH MVC

### 8.1. MODELS

Dự án sử dụng **Code-First** approach với Entity Framework Core. Các model được định nghĩa bằng C# classes và tự động mapping sang database.

| Model Class | Mục đích | Gen từ DB | Tự tạo thêm |
|-------------|----------|-----------|-------------|
| **ApplicationUser.cs** | Mở rộng IdentityUser, lưu thông tin người dùng | | X |
| **Product.cs** | Lưu thông tin sản phẩm (sách): Title, ISBN, Price, StockQty, Description, CoverImageUrl, Status, v.v. | | X |
| **Category.cs** | Lưu thông tin danh mục sách: Name, Slug, Description, BackgroundImageUrl | | X |
| **Author.cs** | Lưu thông tin tác giả: Name, Bio | | X |
| **ProductCategory.cs** | Bảng trung gian many-to-many giữa Product và Category | | X |
| **ProductImage.cs** | Lưu gallery ảnh cho sản phẩm (ngoài ảnh bìa chính) | | X |
| **Order.cs** | Lưu thông tin đơn hàng: OrderNumber, UserId, Status, Total, ShippingAddress, CustomerName, CustomerPhone | | X |
| **OrderItem.cs** | Lưu chi tiết sản phẩm trong đơn hàng: ProductId, Quantity, UnitPrice | | X |
| **CartItem.cs** | Lưu giỏ hàng của user đã đăng nhập | | X |
| **Review.cs** | Lưu đánh giá sản phẩm: Rating (1-5 sao), ProductId, UserId | | X |
| **PaymentProof.cs** | Lưu chứng từ thanh toán: OrderId, ImageUrl, UploadedAt, VerifiedBy, VerifiedAt | | X |
| **Address.cs** | Lưu địa chỉ giao hàng của user | | X |
| **ErrorViewModel.cs** | Model cho trang lỗi | | X |

**Navigation Properties:**
- Product → Author (many-to-one)
- Product → ProductCategories → Category (many-to-many)
- Product → ProductImages (one-to-many)
- Product → Reviews (one-to-many)
- Product → OrderItems (one-to-many)
- Order → User (many-to-one)
- Order → OrderItems (one-to-many)
- Order → PaymentProof (one-to-one)
- Review → Product, User (many-to-one)
- CartItem → Product, User (many-to-one)

### 8.2. VIEWS

#### 8.2.1. Gói Account

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| DangNhap.cshtml | X | | Form đăng nhập |
| DangKy.cshtml | X | | Form đăng ký tài khoản |
| TaiKhoan.cshtml | X | | Hiển thị thông tin tài khoản |
| DonHangCuaToi.cshtml | X | | Lịch sử đơn hàng, form đánh giá |

#### 8.2.2. Gói Admin

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Index.cshtml | X | | Redirect to Dashboard |
| Dashboard.cshtml | X | | Trang tổng quan thống kê |
| Orders.cshtml | X | | Danh sách đơn hàng |
| OrderDetail.cshtml | X | | Chi tiết đơn hàng, xác nhận thanh toán |
| Products.cshtml | X | | Danh sách sản phẩm |
| AddProduct.cshtml | X | | Form thêm sản phẩm |
| EditProduct.cshtml | X | | Form sửa sản phẩm |
| Categories.cshtml | X | | Quản lý danh mục (CRUD inline) |
| Authors.cshtml | X | | Quản lý tác giả (CRUD inline) |
| Customers.cshtml | X | | Danh sách khách hàng |
| CustomerDetail.cshtml | X | | Chi tiết khách hàng |

#### 8.2.3. Gói Cart

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Index.cshtml | X | | Hiển thị giỏ hàng, cập nhật số lượng |

#### 8.2.4. Gói Catalog

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Index.cshtml | X | | Danh sách tất cả danh mục |
| Category.cshtml | X | | Danh sách sản phẩm theo danh mục |

#### 8.2.5. Gói Checkout

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Index.cshtml | X | | Form thanh toán |
| PaymentInstructions.cshtml | X | | Hướng dẫn thanh toán, upload chứng từ |

#### 8.2.6. Gói Home

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Index.cshtml | X | | Trang chủ với hero section, sản phẩm nổi bật |

#### 8.2.7. Gói Product

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| Detail.cshtml | X | | Chi tiết sản phẩm, đánh giá, sản phẩm liên quan |
| Search.cshtml | X | | Tìm kiếm và lọc sản phẩm |

#### 8.2.8. Gói Shared

| Các file View | View | Partial View | Mục đích |
|---------------|------|--------------|----------|
| _Layout.cshtml | X | | Layout chính cho khách hàng |
| _AdminLayout.cshtml | X | | Layout cho admin |
| Error.cshtml | X | | Trang lỗi |
| _ValidationScriptsPartial.cshtml | | X | Scripts validation |

### 8.3. CONTROLLERS

#### 8.3.1. HomeController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Index | Hiển thị trang chủ với 12 sản phẩm mới nhất và danh mục | Index.cshtml |
| Error | Hiển thị trang lỗi | Error.cshtml |

**Business Logic:**
- Lấy sản phẩm Active, sắp xếp theo CreatedAt giảm dần
- Include Author để hiển thị tên tác giả
- Lấy tất cả Category kèm ProductCategories

#### 8.3.2. AccountController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| DangNhap (GET) | Hiển thị form đăng nhập | DangNhap.cshtml |
| DangNhap (POST) | Xử lý đăng nhập | DangNhap.cshtml |
| DangKy (GET) | Hiển thị form đăng ký | DangKy.cshtml |
| DangKy (POST) | Xử lý đăng ký, tự động gán role Customer | DangKy.cshtml |
| DangXuat (POST) | Đăng xuất | Redirect |
| TaiKhoan | Hiển thị thông tin tài khoản | TaiKhoan.cshtml |
| DonHangCuaToi | Hiển thị lịch sử đơn hàng | DonHangCuaToi.cshtml |

**Dependencies:** UserManager, SignInManager, RoleManager, ApplicationDbContext

#### 8.3.3. ProductController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Detail (GET) | Hiển thị chi tiết sản phẩm, đánh giá, sản phẩm liên quan | Detail.cshtml |
| AddReview (POST) | Thêm đánh giá (chỉ cho khách đã mua) | Redirect to Detail |
| Search (GET) | Tìm kiếm, lọc, sắp xếp sản phẩm | Search.cshtml |

**Business Logic:**
- Detail: Include Author, Categories, Images, Reviews
- Tính average rating
- Kiểm tra quyền đánh giá (đã mua & hoàn thành & chưa review)
- Lấy sản phẩm liên quan (cùng category, khác productId)
- Search: Support filter by keyword, price range, category, sort

#### 8.3.4. CatalogController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Index | Danh sách tất cả danh mục | Index.cshtml |
| Category | Danh sách sản phẩm theo danh mục (có phân trang) | Category.cshtml |

**Business Logic:**
- Category: Lọc sản phẩm Active theo CategoryId
- Phân trang 12 items/page
- Include Author để hiển thị

#### 8.3.5. CartController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Index (GET) | Hiển thị giỏ hàng | Index.cshtml |
| AddToCart (POST) | Thêm sản phẩm vào giỏ (JSON API) | N/A |
| UpdateQuantity (POST) | Cập nhật số lượng (JSON API) | N/A |
| RemoveFromCart (POST) | Xóa sản phẩm khỏi giỏ (JSON API) | N/A |
| GetCartCount (GET) | Lấy số lượng trong giỏ (JSON API) | N/A |

**Business Logic:**
- User đã login: lưu trong database (CartItem table)
- User chưa login: lưu trong Session
- Kiểm tra stock availability
- Trả về JSON response cho AJAX calls

#### 8.3.6. CheckoutController

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Index (GET) | Hiển thị form thanh toán | Index.cshtml |
| PlaceOrder (POST) | Xử lý đặt hàng | Redirect to PaymentInstructions |
| PaymentInstructions (GET) | Hướng dẫn thanh toán | PaymentInstructions.cshtml |
| UploadPaymentProof (POST) | Upload chứng từ thanh toán | Redirect to DonHangCuaToi |

**Business Logic:**
- Require Authentication
- Kiểm tra giỏ hàng không rỗng
- Kiểm tra stock availability cho từng item
- Tạo Order với mã tự động ORDyyyyMMddnnn
- Tạo OrderItems
- Trừ stock quantity
- Xóa giỏ hàng
- Upload ảnh: validate file type (JPG, JPEG, PNG), size (max 5MB)
- Lưu vào PaymentProof table

#### 8.3.7. AdminController

**Authorization:** Require role "Admin"

| Action Method | Mục đích | View/Partial View liên quan |
|---------------|----------|----------------------------|
| Index | Redirect to Dashboard | N/A |
| Dashboard | Thống kê tổng quan, biểu đồ | Dashboard.cshtml |
| Orders | Danh sách đơn hàng (có filter status) | Orders.cshtml |
| OrderDetail | Chi tiết đơn hàng | OrderDetail.cshtml |
| VerifyPayment | Xác nhận thanh toán | Redirect |
| UpdateOrderStatus | Cập nhật trạng thái đơn hàng | Redirect |
| Products | Danh sách sản phẩm | Products.cshtml |
| AddProduct (GET) | Form thêm sản phẩm | AddProduct.cshtml |
| CreateProduct (POST) | Xử lý thêm sản phẩm | Redirect |
| EditProduct (GET) | Form sửa sản phẩm | EditProduct.cshtml |
| UpdateProduct (POST) | Xử lý cập nhật sản phẩm | Redirect |
| DeleteProduct (POST) | Soft delete sản phẩm | Redirect |
| Categories | Quản lý danh mục | Categories.cshtml |
| AddCategory (POST) | Thêm danh mục | Redirect |
| EditCategory (POST) | Sửa danh mục | Redirect |
| DeleteCategory (POST) | Xóa danh mục | Redirect |
| Authors | Quản lý tác giả | Authors.cshtml |
| AddAuthor (POST) | Thêm tác giả | Redirect |
| EditAuthor (POST) | Sửa tác giả | Redirect |
| DeleteAuthor (POST) | Xóa tác giả | Redirect |
| Customers | Danh sách khách hàng | Customers.cshtml |
| CustomerDetail | Chi tiết khách hàng | CustomerDetail.cshtml |

**Business Logic:**
- Dashboard: Tính toán thống kê từ DB
  - Total orders, pending orders, total revenue
  - Monthly revenue (12 months)
  - Order status distribution
  - Top 5 selling products
  - Recent 10 orders
  
- Orders: Filter by status, include User, OrderItems, Product, PaymentProof
- VerifyPayment: Update order status to DaXacNhan, set VerifiedBy, VerifiedAt
- UpdateOrderStatus: Validate status, restore stock if cancelled
- Product CRUD: Handle file upload (cover image), manage ProductCategories
- Category CRUD: Handle file upload (background image), cascade delete ProductCategories
- Author CRUD: Prevent delete if has products
- Customers: Calculate order stats for each customer

### 8.4. SƠ ĐỒ ÁNH XẠ VIEW – CONTROLLER – MODEL

| Model | Controller | View | Partial View | Mục đích |
|-------|------------|------|--------------|----------|
| Product | HomeController | Index.cshtml | | Hiển thị sản phẩm nổi bật |
| Product, Author | ProductController | Detail.cshtml | | Chi tiết sản phẩm |
| Product, Author, Category | ProductController | Search.cshtml | | Tìm kiếm sản phẩm |
| Product, Author, Category | CatalogController | Category.cshtml | | Sản phẩm theo danh mục |
| Category | CatalogController | Index.cshtml | | Danh sách danh mục |
| CartItem, Product | CartController | Index.cshtml | | Giỏ hàng |
| Order, OrderItem, Product | CheckoutController | Index.cshtml | | Form thanh toán |
| Order, OrderItem, Product, PaymentProof | CheckoutController | PaymentInstructions.cshtml | | Hướng dẫn thanh toán |
| ApplicationUser | AccountController | DangNhap.cshtml | | Đăng nhập |
| ApplicationUser | AccountController | DangKy.cshtml | | Đăng ký |
| ApplicationUser | AccountController | TaiKhoan.cshtml | | Thông tin tài khoản |
| Order, OrderItem, Product, Review | AccountController | DonHangCuaToi.cshtml | | Lịch sử đơn hàng |
| Order, User, PaymentProof | AdminController | Dashboard.cshtml | | Dashboard admin |
| Order, User, OrderItem, Product, PaymentProof | AdminController | Orders.cshtml | | Quản lý đơn hàng |
| Order, User, OrderItem, Product, PaymentProof | AdminController | OrderDetail.cshtml | | Chi tiết đơn hàng |
| Product, Author, ProductCategory, Category | AdminController | Products.cshtml | | Quản lý sản phẩm |
| Product, Author, Category | AdminController | AddProduct.cshtml | | Thêm sản phẩm |
| Product, Author, Category, ProductCategory | AdminController | EditProduct.cshtml | | Sửa sản phẩm |
| Category, ProductCategory | AdminController | Categories.cshtml | | Quản lý danh mục |
| Author, Product | AdminController | Authors.cshtml | | Quản lý tác giả |
| ApplicationUser, Order | AdminController | Customers.cshtml | | Quản lý khách hàng |
| ApplicationUser, Order, OrderItem, Product | AdminController | CustomerDetail.cshtml | | Chi tiết khách hàng |

---

## 9. LUỒNG HOẠT ĐỘNG CHÍNH

### 9.1. Luồng Khách hàng mua hàng

```
1. Truy cập trang chủ
   ↓
2. Duyệt sản phẩm (Trang chủ / Danh mục / Tìm kiếm)
   ↓
3. Xem chi tiết sản phẩm
   ↓
4. Thêm vào giỏ hàng (có thể thêm nhiều sản phẩm)
   ↓
5. Vào trang giỏ hàng, kiểm tra và cập nhật
   ↓
6. Bấm "Tiến hành thanh toán"
   ↓
7. Đăng nhập (nếu chưa đăng nhập)
   ↓
8. Điền thông tin giao hàng
   ↓
9. Xác nhận đặt hàng
   ↓
10. Chuyển đến trang hướng dẫn thanh toán
    ↓
11. Chuyển khoản và upload chứng từ
    ↓
12. Admin xác nhận thanh toán
    ↓
13. Admin cập nhật trạng thái giao hàng
    ↓
14. Đơn hàng hoàn thành
    ↓
15. Khách hàng đánh giá sản phẩm
```

### 9.2. Luồng Admin quản lý đơn hàng

```
1. Đăng nhập với tài khoản Admin
   ↓
2. Vào trang Dashboard (xem tổng quan)
   ↓
3. Vào "Quản lý đơn hàng"
   ↓
4. Lọc đơn hàng "Chờ thanh toán"
   ↓
5. Click vào đơn hàng để xem chi tiết
   ↓
6. Kiểm tra chứng từ thanh toán
   ↓
7. Bấm "Xác nhận thanh toán"
   → Trạng thái chuyển sang "Đã xác nhận"
   ↓
8. Cập nhật trạng thái "Đang giao"
   ↓
9. Cập nhật trạng thái "Hoàn thành"
```

### 9.3. Luồng Admin quản lý sản phẩm

```
1. Đăng nhập Admin
   ↓
2. Vào "Quản lý sản phẩm"
   ↓
3. Bấm "Thêm sản phẩm mới"
   ↓
4. Điền thông tin:
   - Tiêu đề, ISBN
   - Chọn tác giả (hoặc thêm mới)
   - Chọn danh mục (có thể nhiều)
   - Giá, số lượng
   - Mô tả
   - Upload ảnh bìa
   ↓
5. Bấm "Lưu"
   → Sản phẩm được tạo với slug tự động
   ↓
6. Sản phẩm hiển thị trên trang khách hàng
```

---

## 10. CÔNG NGHỆ VÀ THƯ VIỆN SỬ DỤNG

### 10.1. Backend

- **Framework:** ASP.NET Core MVC (.NET 6/7/8)
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Database:** SQL Server
- **Language:** C# 10+

### 10.2. Frontend

- **UI Framework:** Bootstrap 5.3.2
- **Icons:** Bootstrap Icons 1.11.1
- **JavaScript:** Vanilla JavaScript (ES6+)
- **AJAX:** Fetch API
- **3D Graphics:** Spline (iframe embed)

### 10.3. Patterns & Architecture

- **Architecture:** MVC (Model-View-Controller)
- **Data Access:** Repository pattern (thông qua DbContext)
- **Authentication:** Cookie-based authentication
- **Authorization:** Role-based authorization (Admin, Customer)
- **Session Management:** ASP.NET Core Session (cho giỏ hàng guest)

---

## 11. TÍNH NĂNG NỔI BẬT

### 11.1. Cho Khách hàng

1. **Tìm kiếm & Lọc nâng cao:**
   - Tìm kiếm theo tên sách, tác giả
   - Lọc theo giá, danh mục
   - Sắp xếp đa dạng

2. **Giỏ hàng linh hoạt:**
   - Lưu giỏ hàng cho cả guest và user
   - Cập nhật realtime bằng AJAX
   - Badge hiển thị số lượng

3. **Đánh giá sản phẩm:**
   - Chỉ cho phép khách đã mua
   - Rating 1-5 sao
   - Hiển thị điểm trung bình

4. **Quy trình thanh toán rõ ràng:**
   - Form nhập địa chỉ chi tiết
   - Hướng dẫn chuyển khoản
   - Upload chứng từ

### 11.2. Cho Admin

1. **Dashboard trực quan:**
   - Thống kê doanh thu, đơn hàng
   - Biểu đồ trực quan
   - Top sản phẩm bán chạy

2. **Quản lý đơn hàng hiệu quả:**
   - Lọc theo trạng thái
   - Xác nhận thanh toán
   - Cập nhật trạng thái giao hàng
   - Tự động hoàn trả stock khi hủy

3. **Quản lý sản phẩm toàn diện:**
   - CRUD sản phẩm
   - Upload ảnh
   - Quản lý danh mục, tác giả
   - Soft delete

---

## 12. BẢO MẬT

### 12.1. Authentication & Authorization

- Sử dụng ASP.NET Core Identity (industry standard)
- Password hashing (SHA256 + salt)
- Role-based authorization
- Anti-forgery token cho POST requests

### 12.2. Validation

- Server-side validation (Model validation)
- Client-side validation (jQuery Validation)
- File upload validation (type, size)
- Input sanitization

### 12.3. Session & Cookie

- Cookie-based authentication
- Secure session management
- HttpOnly cookies

---

## 13. ĐIỂM MẠNH & ĐIỂM CẦN CẢI THIỆN

### 13.1. Điểm mạnh

✅ **Kiến trúc rõ ràng:** MVC pattern được áp dụng đúng chuẩn  
✅ **UI/UX hiện đại:** Bootstrap 5.3, responsive design  
✅ **Bảo mật tốt:** Identity, role-based, anti-forgery  
✅ **Code sạch:** Separation of concerns, naming conventions  
✅ **Chức năng đầy đủ:** CRUD cho tất cả entities  
✅ **User experience:** AJAX, validation, toast notifications  

### 13.2. Điểm cần cải thiện

⚠️ **Performance:**
- Chưa có caching
- Chưa optimize queries (N+1 problem)
- Chưa có CDN cho static files

⚠️ **Scalability:**
- File upload lưu local (nên dùng cloud storage)
- Session-based cart cho guest (nên dùng distributed cache)

⚠️ **Features:**
- Chưa có chức năng "Quên mật khẩu"
- Chưa có email notification
- Chưa có comment trong review (chỉ có rating)
- Chưa có wishlist
- Chưa có so sánh sản phẩm

⚠️ **SEO:**
- Chưa có meta tags cho SEO
- Chưa có sitemap.xml
- Chưa có structured data

⚠️ **Payment:**
- Chỉ hỗ trợ chuyển khoản (manual verification)
- Chưa tích hợp payment gateway (VNPay, MoMo, v.v.)

---

## 14. KẾT LUẬN

Ứng dụng **DOAN_Web - Shopjoy** là một hệ thống bán sách trực tuyến được xây dựng hoàn chỉnh với đầy đủ chức năng cơ bản của một trang thương mại điện tử. Ứng dụng áp dụng đúng mô hình **MVC**, có cấu trúc code rõ ràng, dễ bảo trì và mở rộng.

**Các điểm nổi bật:**
- Phân quyền rõ ràng giữa Admin và Customer
- Quy trình mua hàng trực quan, dễ sử dụng
- Giao diện hiện đại, responsive
- Bảo mật tốt với ASP.NET Core Identity

Ứng dụng đã sẵn sàng để triển khai và sử dụng trong môi trường production với một số cải tiến về performance và tính năng bổ sung như đã đề xuất ở phần 13.2.

---

**Người thực hiện:** [Tên sinh viên]  
**Ngày hoàn thành:** [Ngày tháng năm]  
**Lớp:** [Tên lớp]  
**Giảng viên hướng dẫn:** [Tên giảng viên]
