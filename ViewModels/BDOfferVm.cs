using System.ComponentModel.DataAnnotations;

namespace Vision.ViewModels
{
    public class BDOfferVm
    {
        [Required(ErrorMessage = "TitleAr Required")]
        public string TitleAr { get; set; }
        [Required(ErrorMessage = "TitleEn Required")]
        public string TitleEn { get; set; }
        public string OfferDescription { get; set; }
        [Required(ErrorMessage = "Price Required")]
        public double Price { get; set; }
        public long ClassifiedBusinessId { get; set; }
    }
}
