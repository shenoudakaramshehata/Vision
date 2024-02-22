using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Interest
    {
        [Key]
        public int InterestID { get; set; }
        public string InterestTitle { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

    }
}
