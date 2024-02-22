using Vision.Models;

namespace Vision.ViewModels
{
    public class ShoppingCartVM
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public double ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public float ProductTotal { get; set; }
        public int? ProductPriceId { get; set; }
        public List<ShopingCartProductExtraFeatures>? ShopingCartProductExtraFeatures { get; set; }
        
    }
}
