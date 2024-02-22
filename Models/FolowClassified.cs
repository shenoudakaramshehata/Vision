using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class FolowClassified
    {
        [Key]
        public int FolowClassifiedId { get; set; }
        public string UserId { get; set; }
        public long ClassifiedAdId { get; set; }
        [JsonIgnore]
        public virtual ClassifiedAd ClassifiedAd { get; set; }
    }
}
