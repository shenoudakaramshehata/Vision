namespace Vision.ViewModels
{
    public class NewClassifiedAdsVM
    {
        public string UseId { get; set; }
        public bool IsActive { get; set; }
        public int ClassifiedAdsCategoryId { get; set; }
        public List<AddContentVM> addContentVMs { get; set; }
        public List<MediaVm> mediaVms { get; set; }
    }
}
