using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class EntityType
    {
        [Key]
        public int EntityTypeId { get; set; }
        public string EntityTitleAr { get; set; }
        public string EntityTitleEn { get; set; }
        public virtual ICollection<Banner>Banners { get; set; }
    }
}
