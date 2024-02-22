using System.ComponentModel.DataAnnotations.Schema;

namespace Vision.Models
{
    public class WalletSubscription
    {
        public int WalletSubscriptionId { get; set; }
        public DateTime SubscriptionDate { get; set; }
        
        public string UserId { get; set; }
        public int WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        public string? PaymentID { get; set; }
        public bool IsPaid { get; set; }
        public int PaymentMethodId { get; set; }
        
}
}
