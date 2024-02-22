namespace Vision.Models
{
    public class ServiceContent
    {
        public ServiceContent()
        {
            ServiceContentValues = new HashSet<ServiceContentValue>();
        }

        public long ServiceContentId { get; set; }
        public int ServiceTemplateConfigId { get; set; }
        public long ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceTemplateConfig ServiceTemplateConfig { get; set; }
        public virtual ICollection<ServiceContentValue> ServiceContentValues { get; set; }
    }
}
