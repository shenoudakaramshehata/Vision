using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class ProductImage
    {
        [Key]
        public long ProductImageId { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string Image { get; set; }
    }
}
