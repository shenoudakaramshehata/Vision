namespace Vision.Models
{
    public class ServiceContentValue
    {
        public long ServiceContentValueId { get; set; }
        public long ServiceContentId { get; set; }
        public string? ContentValue { get; set; }

        public virtual ServiceContent? ServiceContent { get; set; }
    }
}
