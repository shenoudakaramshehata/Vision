namespace Vision.Models
{
    public class ProductExtra
    {
        public int ProductExtraId { get; set; }
        public double Price { get; set; }
        public string ExtraTitleAr { get; set; }
        public string ExtraTitleEn { get; set; }
        public string ExtraDes { get; set; }
        public long ProductId { get; set; }
        public virtual  Product Product { get; set; }
       
    }
}
