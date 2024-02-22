using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace Vision.Models
{
    public class BusiniessSubscription
    {
        [Key]
        public int BusiniessSubscriptionId { get; set; }
        public int BussinessPlanId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Price { get; set; }
        public string Remarks { get; set; }
        public int? PaymentMethodId { get; set; }
        public bool IsActive { set; get; }
        public string PaymentID { set; get; }
        public string UserId { get; set; }

        public long? ClassifiedBusinessId { get; set; }
        [JsonIgnore]
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
       
        public virtual BussinessPlan BussinessPlan { get; set; }
        public virtual PaymentMehod PaymentMethod { get; set; }
    }
}
