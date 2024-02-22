using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class ServiceQuotation
    {
        [Key]
        public int ServiceQuotationId { get; set; }
        public DateTime QuotationDate { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public long BDId { get; set; }
        public long ServiceId { get; set; }
        public int ServiceQuotationRequestStatusId { get; set; }
        [JsonIgnore]
        public virtual Service Service { get; set; }
        [JsonIgnore]
        public virtual ServiceQuotationRequestStatus ServiceQuotationRequestStatus { get; set; }
    }
}
