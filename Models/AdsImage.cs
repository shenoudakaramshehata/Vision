using System.ComponentModel.DataAnnotations;
namespace Vision.Models
{
    public class AdsImage
    {
        [Key]
        public long AdsImageId { get; set; }
        public long ClassifiedAdId { get; set; }
        public virtual ClassifiedAd ClassifiedAd { get; set; }
        public string Image { get; set; }
    }
}
