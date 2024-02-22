using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class BDOffer
    {
        [Key]
        public int BDOfferId { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string OfferDescription { get; set; }
        public string Pic { get; set; }
        public double Price { get; set; }
        public DateTime PublishDate { get; set; }
        public virtual ICollection<BDOfferImage> BDOfferImages { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }

    }
}
