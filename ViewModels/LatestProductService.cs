namespace Vision.ViewModels
{
    public class LatestProductService
    {
        public long Id { get; set; }
        public double Price { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string CatagoryTitleAr { get; set; }
        public string CatagoryTitleEn { get; set; }
        public string MainPic { get; set; }
        public string Description { get; set; }
        public bool IsFavourite { get; set; }
        public DateTime Publ { get; set; }
        public int TypeId { get; set; }
    }
}
