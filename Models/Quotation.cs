using Vision.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vision.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public DateTime QuotationDate { get; set; }
        public string Description { get; set; }
       
        public string UserId { get; set; }
        public long ClassifiedAdId { get; set; }
        [JsonIgnore]
        public virtual ClassifiedAd ClassifiedAd { get; set; }


    }
}
