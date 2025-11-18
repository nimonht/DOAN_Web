# Hướng dẫn sử dụng Báo cáo Phân tích Ứng dụng

## Tệp báo cáo

**File chính:** `BAO_CAO_PHAN_TICH_UNG_DUNG.md`

## Nội dung báo cáo

Báo cáo phân tích toàn diện hệ thống bán sách trực tuyến **DOAN_Web - Shopjoy**, bao gồm:

### 1. Tổng quan ứng dụng
- Công nghệ sử dụng: ASP.NET Core MVC, Entity Framework Core, Identity
- Mô hình kiến trúc: MVC
- Database: SQL Server

### 2. Phân tích Layout và Chức năng (Phần 6)

#### 6.1 Phần Admin
- Layout Admin (_AdminLayout.cshtml)
- Dashboard với thống kê và biểu đồ
- Quản lý Đơn hàng
- Quản lý Sản phẩm
- Quản lý Danh mục
- Quản lý Tác giả
- Quản lý Khách hàng

#### 6.2 Phần Khách hàng
- Master Layout (_Layout.cshtml)
- Trang Home
- Danh sách Sản phẩm (Category & Search)
- Chi tiết Sản phẩm
- Giỏ hàng
- Thanh toán
- Xác nhận Đơn hàng / Hướng dẫn thanh toán
- Lịch sử Mua hàng
- Đăng ký / Đăng nhập

### 3. Tổng kết Chức năng theo Vai trò (Phần 7)
Bảng phân quyền chi tiết cho từng model/action method theo vai trò Admin và Customer.

### 4. Mô tả chi tiết theo MVC (Phần 8)

#### 8.1 Models (13 models)
- ApplicationUser
- Product
- Category
- Author
- ProductCategory
- ProductImage
- Order
- OrderItem
- CartItem
- Review
- PaymentProof
- Address
- ErrorViewModel

#### 8.2 Views (8 gói views)
- Account (4 views)
- Admin (11 views)
- Cart (1 view)
- Catalog (2 views)
- Checkout (2 views)
- Home (1 view)
- Product (2 views)
- Shared (4 views)

#### 8.3 Controllers (7 controllers)
- HomeController
- AccountController
- ProductController
- CatalogController
- CartController
- CheckoutController
- AdminController

#### 8.4 Sơ đồ ánh xạ View-Controller-Model
Bảng mapping chi tiết giữa Model, Controller và View.

### 5. Luồng hoạt động chính (Phần 9)
- Luồng khách hàng mua hàng (15 bước)
- Luồng admin quản lý đơn hàng (9 bước)
- Luồng admin quản lý sản phẩm (6 bước)

### 6. Công nghệ và Thư viện (Phần 10)
- Backend: ASP.NET Core MVC, EF Core, Identity
- Frontend: Bootstrap 5.3, Bootstrap Icons, Vanilla JS
- Patterns: MVC, Repository, Role-based Authorization

### 7. Tính năng nổi bật (Phần 11)
- Cho khách hàng: Tìm kiếm nâng cao, Giỏ hàng linh hoạt, Đánh giá sản phẩm
- Cho admin: Dashboard trực quan, Quản lý đơn hàng hiệu quả

### 8. Bảo mật (Phần 12)
- Authentication & Authorization với Identity
- Validation (server-side & client-side)
- Session & Cookie security

### 9. Điểm mạnh & Điểm cần cải thiện (Phần 13)
- Điểm mạnh: Kiến trúc rõ ràng, UI/UX hiện đại, Bảo mật tốt
- Điểm cần cải thiện: Performance, Scalability, Features, SEO, Payment

### 10. Kết luận (Phần 14)

## Cách xem báo cáo

### Trên GitHub
1. Mở file `BAO_CAO_PHAN_TICH_UNG_DUNG.md` trên GitHub
2. GitHub sẽ tự động render Markdown với định dạng đẹp

### Trên máy tính
1. Clone repository
2. Mở file bằng VS Code, Notepad++, hoặc bất kỳ text editor nào hỗ trợ Markdown
3. Hoặc dùng Markdown viewer như Typora, MarkText

### Xuất PDF (nếu cần)
1. Mở file trong VS Code
2. Cài extension "Markdown PDF"
3. Right-click → "Markdown PDF: Export (pdf)"

## Thông tin thêm

- **Tổng số dòng:** 994 dòng
- **Ngôn ngữ:** Tiếng Việt
- **Định dạng:** Markdown (.md)
- **Sẵn sàng nộp:** Có

## Ghi chú

Các thông tin cá nhân ở cuối báo cáo (Tên sinh viên, Lớp, Giảng viên) cần được điền vào trước khi nộp.

Báo cáo này được tạo tự động dựa trên phân tích code của ứng dụng DOAN_Web.
