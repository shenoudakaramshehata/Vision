using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class BDOfferImage
    {
        [Key]
        public int BDOfferImageId { get; set; }
        public int BDOfferId { get; set; }
        public virtual BDOffer BDOffer { get; set; }
        public string Image { get; set; }

    }
}
