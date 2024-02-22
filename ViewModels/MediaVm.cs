namespace Vision.ViewModels
{
    public class MediaVm
    {
        public int AdTemplateConfigId { get; set; }
        public IFormFileCollection Media { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
