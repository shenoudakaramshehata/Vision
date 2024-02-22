using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        //[JsonIgnore]
        //public virtual AddListing? AddListing { get; set; }
        //public int? AddListingId { get; set; }
        [JsonIgnore]
        public virtual ClassifiedBusiness? ClassifiedBusiness { get; set; }
        public long? ClassifiedBusinessId { get; set; }

    }
}
