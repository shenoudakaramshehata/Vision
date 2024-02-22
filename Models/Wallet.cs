using System.ComponentModel.DataAnnotations;

namespace Vision.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }
        public string WalletTitleAr { get; set; }
        public string WalletTitleEn { get; set; }
        public bool IsActive { get; set; }
        public int NumberOfClassifed { get; set; }
        public int SortOrder { get; set; }
        public double Price { get; set; }
        public virtual ICollection<WalletSubscription> WalletSubscriptions { get; set; }


    }
}
