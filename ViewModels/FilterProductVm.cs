namespace Vision.ViewModels
{
    public class FilterProductVm
    {
        public long ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public double FromPrice { get; set; }
        public double ToPrice { get; set; }
        public List<ProductContentModelVm> productContentModelVms { get; set; }

    }
}
