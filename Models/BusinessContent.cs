namespace Vision.Models
{
    public class BusinessContent
    {
        public BusinessContent()
        {
            BusinessContentValues = new HashSet<BusinessContentValue>();
        }

        public long BusinessContentId { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public int BusinessTemplateConfigId { get; set; }

        public virtual BusinessTemplateConfig BusinessTemplateConfigs { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
        public virtual ICollection<BusinessContentValue> BusinessContentValues { get; set; }
    }
}
