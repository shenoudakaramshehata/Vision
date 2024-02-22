using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class OrderItemExtraProduct
    {
        [Key]
        public int OrderItemExtraProductId { get; set; }
        public int? OrderItemId { get; set; }
        public int? ProductExtraId { get; set; }
        public double Price { get; set; }
        public virtual OrderItem OrderItem { get; set; }
        public virtual ProductExtra ProductExtra { get; set; }
        
    }
}
