namespace Vision.ViewModels
{
    public class AddProductVm
    {
        public bool IsActive { get; set; }
        public bool IsFixedPrice { get; set; }
        public double? Price { get; set; }
        public string ProductName { get; set; }

        public int SortOrder { get; set; }
        public long ProductTypeId { get; set; }
        public long ProductCategoryId { get; set; }
        public List<AddContentVM> addContentVMs { get; set; }
        public List<MediaVm> mediaVms { get; set; }
    }
}
