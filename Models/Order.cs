using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Vision.Models
{
    
    public class Order
    {
        
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CustomerAddressId { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public bool IsDeliverd { get; set; }
        public string OrderSerial { get; set; }
        public string? OrderNotes { get; set; }
        public double OrderTotal { get; set; }
        public bool IsCancelled { get; set; }
        public double? Deliverycost { get; set; }
        public double? OrderNet { get; set; }
        public double? Discount { get; set; }
        public bool IsPaid { get; set; }
        public int UniqeId { get; set; }
        /// <summary>
        /// Address
       
        public string Adress { get; set; }
        public string Governorate { get; set; }
        public string Area { get; set; }
        public string Piece { get; set; }
        public string Avenue { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string ApartmentNumber { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string UserId { get; set; }
       
        [JsonIgnore]
        public virtual CustomerAddress CustomerAddress { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    
       
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
       


    }
}