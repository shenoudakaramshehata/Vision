namespace Vision.ViewModels
{
    public class EditBDVm
    {
        public long BDId { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public string Location { get; set; }
        public List<BusinessContentVM> BusinessContentVMS { get; set; }
    }
}
