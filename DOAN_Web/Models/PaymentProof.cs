using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.Models
{
    public class PaymentProof
    {
        [Key]
        public int PaymentProofId { get; set; }
        public int OrderId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
