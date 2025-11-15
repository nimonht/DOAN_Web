using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ chi tiết")]
        [StringLength(200)]
        public string AddressLine2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn thành phố")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phường/xã")]
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}
