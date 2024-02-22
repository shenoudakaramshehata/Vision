namespace Vision.ViewModels
{
    public class EditBannerVm
    {
        public int BannerId { get; set; }
        public string BannerPic { get; set; }
        public string LargePic { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public int EntityTypeId { get; set; }
        public bool BannerIsActive { get; set; }
        public int BannerOrderIndex { get; set; }
    }
}
