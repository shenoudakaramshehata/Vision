namespace Vision.ViewModels
{
    public class ProductMediaVm
    {
        public int ProductTemplateConfigId { get; set; }
        public IFormFileCollection? Media { get; set; }
        public List<IFormFile>? FileList { get; set; }
    }
}
