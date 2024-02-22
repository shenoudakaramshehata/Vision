namespace Vision.ViewModels
{
    public class ServiceVm
    {
        public string ServiceTitleAr { get; set; }
        public string ServiceTitleEn { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public double Price { get; set; }
        public long ServiceCatagoryId { get; set; }
        public long ServiceTypeId { get; set; }
        public string Description { get; set; }
        public List<ServiceContentVm>serviceContentVms { get; set; }
    }
}
