using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Video
    {
        [Key]
        public int VideoID { get; set; }
        public string VideoT { get; set; }
        public string Caption { get; set; }
        public DateTime PublishDate { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
    }
}
