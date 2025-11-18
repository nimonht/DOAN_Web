using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.ViewModels
{
    public class ReviewInputModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Vui lòng chọn mức đánh giá từ 1 đến 5 sao.")]
        public int Rating { get; set; }
    }
}
