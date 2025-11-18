using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Vui lòng chọn mức đánh giá từ 1 đến 5 sao.")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
