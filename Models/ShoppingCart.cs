using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Vision.Models
{
    
    public  class ShoppingCart
    {
        [Key]
        public int ShoppingCartId { get; set; }
        public long ProductId { get; set; }
        public double ItemPrice { get; set; }
        public int ProductQty { get; set; }
        public double ProductTotal { get; set; }
        public double Deliverycost { get; set; }
       
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [ForeignKey("ProductPrice")]
        public int? ProductPriceId { get; set; }
        //[ForeignKey("ProductPriceId")]
        public virtual ProductPrice? ProductPrice { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<ShopingCartProductExtraFeatures> ShopingCartProductExtraFeatures { get; set; }

    }
}