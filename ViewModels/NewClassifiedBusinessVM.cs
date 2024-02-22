namespace Vision.ViewModels
{
    public class NewClassifiedBusinessVM
    {
        public string UseId { get; set; }
        public bool IsActive { get; set; }
        public int BusinessCategoryId { get; set; }
        public List<BusinessContentVM> addContentVMs { get; set; }
        public List<BusinessMediaVm> mediaVms { get; set; }
    }
}
