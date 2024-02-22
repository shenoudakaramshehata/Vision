using System.ComponentModel.DataAnnotations;
namespace Vision.Models
{
    public class BDImage
    {
        [Key]
        public long BDImageId { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
        public string Image { get; set; }
    }
}
