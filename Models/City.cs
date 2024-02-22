using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class City
    {


        public int CityId { get; set; }
        public string CityTlAr { get; set; }
        public string CityTlEn { get; set; }
        public bool CityIsActive { get; set; }
        public int CityOrderIndex { get; set; }
        [JsonIgnore]
        public virtual ICollection<Area> Area { get; set; }

    }
}
