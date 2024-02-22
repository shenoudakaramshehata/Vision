using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Education
    {
        [Key]
        public int EducationID { get; set; }
        //[Required(ErrorMessage = "Is Required"),MinLength(4,ErrorMessage ="Min Length Is 4")]
        public int Year { get; set; }
        //[Required(ErrorMessage = "Is Required")]
        public string Provider { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
    }
}
