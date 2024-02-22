namespace Vision.ViewModels
{
    public class CheckOutVM
    {
        public string UserId { get; set; }
        public string? Address { get; set; }

        public bool FastOrder { get; set; }
        public int CustomerAddressId { get; set; }
        //public CustomerAddressVM customerAddressVM { get; set; }

    }
}
