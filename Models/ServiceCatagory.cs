namespace Vision.Models
{
    public class ServiceCatagory
    {
        public long ServiceCatagoryId { get; set; }
        public string ServiceCatagoryTitleAr { get; set; }
        public string ServiceCatagoryTitleEn { get; set; }
        public string Pic { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
