using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Vision.Models
{
    public class ShopingCartProductExtraFeatures
    {
        [Key]
        public int ShopingCartProductExtraFeaturesId { get; set; }
       
        public int ProductExtraId { get; set; }
        public long ProductId { get; set; }
        public double Price { get; set; }
        public virtual ProductExtra ProductExtra { get; set; }
        public int ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
        

    }
}
