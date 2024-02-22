using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Vision.Models
{
    public class ServiceFavourite
    {
        [Key]
        public int ServiceFavouriteId { get; set; }
        public string UserId { get; set; }
        public long ServiceId { get; set; }
        [JsonIgnore]
        public virtual Service Service { get; set; }
    }
}
