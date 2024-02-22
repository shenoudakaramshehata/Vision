namespace Vision.ViewModels
{
    public class ClassifiedAdsVM
    {
        public string UseId { get; set; }
        public bool IsActive { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public double Price { get; set; }
		public string PhoneNumber { get; set; }
		public string Description { get; set; }
		public string Location { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public int ClassifiedAdsCategoryId { get; set; }
        public List<AddContentVM> addContentVMs { get; set; }

    }
}
