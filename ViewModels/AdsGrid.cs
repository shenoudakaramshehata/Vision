namespace Vision.ViewModels
{
    public class AdsGrid
    {
		public long ClassifiedAdId { get; set; }
		public int ClassifiedAdsCategoryId { get; set; }
		public bool Active { get; set; }
        
		public DateTime PublishDate { get; set; }
		public string User { get; set; }
        public int? Views { get; set; }
        public string ClassifiedAdsCategoryTitleAr { get; set; }
        public string ClassifiedAdsCategoryTitleEn { get; set; }

        public string TitleAr { get; set; }
        public string TitleEn { get; set; }

        public double Price { get; set; }
        public string MainPic { get; set; }

        public string Description { get; set; }

        public string City { get; set; }
        public string Area { get; set; }

       
    }
}
