using System.ComponentModel.DataAnnotations;

namespace Vision.ViewModels
{
    public class AddClassifiedAdVm
    {
        [Required(ErrorMessage ="Ad Arabic Title Is Required")]
        public string TitleAr { get; set; }
        [Required(ErrorMessage = "Ad English Title Is Required")]
        public string TitleEn { get; set; }
        [Required(ErrorMessage = "Ad Price Is Required")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Ad Description Is Required")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
    }
}
