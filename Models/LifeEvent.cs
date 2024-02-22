using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class LifeEvent
    {
        [Key]
        public int LifeEventID { get; set; }
        public string EventType { get; set; }
        public string Caption { get; set; }
        public string Details { get; set; }
        public string Media { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
    }
}
