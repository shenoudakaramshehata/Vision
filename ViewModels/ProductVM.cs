namespace Vision.ViewModels
{
    public class ProductVM
    {
        public bool IsActive { get; set; }
        public double Price { get; set; }
        public bool IsFixedPrice { get; set; }
        //public string ProductName { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string Description { get; set; }
        //public string Location { get; set; }
        public int SortOrder { get; set; }
        public long ProductTypeId { get; set; }
        public long ProductCategoryId { get; set; }
        public List<ProductContentVM>? ProductContentVMS { get; set; }

    }
}
