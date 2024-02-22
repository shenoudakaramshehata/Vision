using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string? CountryTlAr { get; set; }
        public string? CountryTlEn { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<City>? City { get; set; }
    }
}
