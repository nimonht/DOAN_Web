using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public virtual Product Product { get; set; } = null!;
        public virtual ApplicationUser? User { get; set; }
    }
}
