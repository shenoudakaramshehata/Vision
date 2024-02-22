using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Folwers
    {
        public int FolwersId { get; set; }
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual AddListing AddListing { get; set; }
        public int AddListingId { get; set; }


    }
}
