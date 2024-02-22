
using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class ProductPrice
    {
        [Key]
        public int ProductPriceId { get; set; }
        public string ProductPriceTilteAr { get; set; }
        public string ProductPriceTilteEn { get; set; }
       
        public double Price { get; set; }
        public string ProductPriceDes { get; set; }
        public long? ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
