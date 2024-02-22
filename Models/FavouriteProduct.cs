using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class FavouriteProduct
    {
        [Key]
        public int FavouriteProductId { get; set; }
        public string UserId { get; set; }
        public long ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
