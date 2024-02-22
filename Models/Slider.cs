using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public  class Slider
    {
        [Key]
        public int SliderId { get; set; }
        public string Pic { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int OrderIndex { get; set; }
        public int PageIndex { get; set; }
        public string EntityId { get; set; }
        public int EntityTypeId { get; set; }
        public virtual EntityType EntityType { get; set; }
       
    }
}