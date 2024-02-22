
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Vision.Models
{
    public class FolowProfile
    {
        [Key]
        public int FolowProfileId { get; set; }
        public string UserId { get; set; } 
        public string Id { get; set; }
    }
}
