namespace Vision.Models
{
    public class ProductContent
    {
        public ProductContent()
        {
            ProductContentValues = new HashSet<ProductContentValue>();
        }

        public long ProductContentId { get; set; }
        public int ProductTemplateConfigId { get; set; }
        public long ProductId { get; set; }
        public virtual Product Product  { get; set; }
        public virtual ProductTemplateConfig ProductTemplateConfigs { get; set; }
        public virtual ICollection<ProductContentValue> ProductContentValues { get; set; }
    }
}
