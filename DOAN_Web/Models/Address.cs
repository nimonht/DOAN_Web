using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
