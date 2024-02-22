using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class ServiceImage
    {
        [Key]
        public long ServiceImageId { get; set; }
        public long ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public string Image { get; set; }
    }
}
