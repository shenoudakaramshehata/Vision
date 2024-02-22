using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class BussinessPlan
    {
        [Key]
        public int BussinessPlanId { get; set; }
        public string PlanTlAr { get; set; }
        public string PlanTlEn { get; set; }
        public bool? IsActive { get; set; }
        public double? Price { get; set; }
        public int? DurationInMonth { get; set; }
        
        public virtual ICollection<BusiniessSubscription> BusiniessSubscriptions { get; set; }

    }
}
