namespace Vision.ViewModels
{
    public class BusinessVM
    {
        public string? UseId { get; set; }
        public bool IsActive { get; set; }
        public int BusinessCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int PlanId { get; set; }
        public int PaymentMethodId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string phone { get; set; }
        public List<BusinessContentVM>? BusinessContentVMS { get; set; }
    }
}
