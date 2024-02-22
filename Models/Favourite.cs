using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Favourite
    {
        [Key]
        public int FavouriteId { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual AddListing AddListing { get; set; }
        public int AddListingId { get; set; }
    }
}
