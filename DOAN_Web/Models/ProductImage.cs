using System.ComponentModel.DataAnnotations;

namespace DOAN_Web.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
