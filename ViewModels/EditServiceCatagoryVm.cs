namespace Vision.ViewModels
{
    public class EditServiceCatagoryVm
    {
        public long ServiceCatagoryId { get; set; }
        public string ServiceCatagoryTitleAr { get; set; }
        public string ServiceCatagoryTitleEn { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
