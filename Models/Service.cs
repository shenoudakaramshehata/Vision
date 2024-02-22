namespace Vision.Models
{
    public class Service
    {
        public long ServiceId { get; set; }
        public string ServiceTitleAr { get; set; }
        public string ServiceTitleEn { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public double Price { get; set; }
        public string MainPic { get; set; }
        public string Reel { get; set; }
        public string Description { get; set; }
        public long ServiceCatagoryId { get; set; }
        public long ServiceTypeId { get; set; }
        public virtual ServiceCatagory? ServiceCatagory { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<ServiceContent> ServiceContents { get; set; }
        public virtual ICollection<ServiceImage> ServiceImages { get; set; }
        public virtual ICollection<ServiceQuotation> ServiceQuotations { get; set; }


    }
}
