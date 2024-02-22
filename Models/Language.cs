using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Language
    {
        [Key]
        public int LanguageID { get; set; }
        public string LanguageTitle { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
    }
}
