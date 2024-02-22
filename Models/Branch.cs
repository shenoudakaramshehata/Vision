using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class Branch
    {
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string Title { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        [JsonIgnore]
        public virtual AddListing AddListing { get; set; }
        public int AddListingId { get; set; }

    }
}
