namespace Vision.Models
{
    public class BusinessContentValue
    {
        public long BusinessContentValueId { get; set; }
        public long BusinessContentId { get; set; }
        public string? ContentValue { get; set; }

        public virtual BusinessContent? BusinessContent { get; set; }
    }
}
