namespace Vision.ViewModels
{
    public class AdzDetails
    {
        public long ClassifiedAdId { get; set; }
        public DateTime PublishDate { get; set; }
        public string UseId { get; set; }
        public int Views { get; set; }
        public bool IsActive { get; set; }
        public string ClassifiedAdsCategoryTitleAr { get; set; }
        public string ClassifiedAdsCategoryTitleEn { get; set; }
        public List<AdzDetailsContent> AdContent { get; set; }
    }
}
