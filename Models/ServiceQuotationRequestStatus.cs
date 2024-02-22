using System.ComponentModel.DataAnnotations;
namespace Vision.Models
{
    public class ServiceQuotationRequestStatus
    {
        [Key]
        public int ServiceQuotationRequestStatusId { get; set; }
        public string StatusTitleAr { get; set; }
        public string StatusTitleEn { get; set; }
       
    }
}
