using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class FavouriteBusiness
    {
        [Key]
        public int FavouriteClassifiedId { get; set; }
        public string? UserId { get; set; }
        public long ClassifiedBusinessId { get; set; }
        [JsonIgnore]
        public virtual ClassifiedBusiness? ClassifiedBusiness { get; set; }
    }
}
