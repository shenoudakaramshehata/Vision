namespace Vision.Models
{
    public class ServiceMediaVm
    {
        public int ServiceTemplateConfigId { get; set; }
        public IFormFileCollection Media { get; set; }
    }
}
