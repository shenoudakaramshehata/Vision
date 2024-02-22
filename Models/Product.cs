using Vision.ViewModels;

namespace Vision.Models
{
    public class Product
    {
        public Product()
        {
            ProductContents = new HashSet<ProductContent>();
            ProductExtras = new HashSet<ProductExtra>();
            ProductPrices = new HashSet<ProductPrice>();
        }
        public long ProductId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFixedPrice { get; set; }
        public double? Price { get; set; }
        //public string? ProductName { get; set; }
        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string MainPic { get; set; }
		public string Reel { get; set; }
		//public string Location { get; set; }
		public string Description { get; set; }
		public int SortOrder { get; set; }
        public long ProductCategoryId { get; set; }
        public long ProductTypeId { get; set; }
        public virtual ProductCategory? ProductCategory { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<ProductContent> ProductContents { get; set; }
        public virtual ICollection<ProductExtra> ProductExtras { get; set; }
        public virtual ICollection<ProductPrice>? ProductPrices { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }

    }
}
