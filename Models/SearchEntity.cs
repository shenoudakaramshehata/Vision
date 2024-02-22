using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class SearchEntity
    {
        [Key]
        public long SearchEntityId { get; set; }
        public int ClassifiedAdsCatagoryId { get; set; }
        public int SearchCatagoryLevel { get; set; }

    }
}
