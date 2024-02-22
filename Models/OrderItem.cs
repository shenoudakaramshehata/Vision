using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public long ProductId { get; set; }
        public double ItemPrice { get; set; }
        public int ProductQuantity { get; set; }
        public double Total { get; set; }
        public int? ProductPriceId { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [ForeignKey("ProductPriceId")]
        public virtual ProductPrice ProductPrice { get; set; }
        public virtual ICollection<OrderItemExtraProduct>? OrderItemExtraProducts { get; set; }

    }
}
