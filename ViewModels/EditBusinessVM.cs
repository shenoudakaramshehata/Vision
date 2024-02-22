namespace Vision.ViewModels
{
    public class EditBusinessVM
    {
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<BusinessContentVM>? BusinessContentVMS { get; set; }
    }
}
