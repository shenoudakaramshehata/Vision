
#nullable disable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Area
    {
       
        public int AreaId { get; set; }
        public int CityId { get; set; }
        public string AreaTlAr { get; set; }
        public string AreaTlEn { get; set; }
        public bool AreaIsActive { get; set; }
        public int AreaOrderIndex { get; set; }
        [JsonIgnore]
        public virtual City City { get; set; }
       
    }
}