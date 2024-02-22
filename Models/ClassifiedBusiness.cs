using System.Text.Json.Serialization;

namespace Vision.Models
{
    public partial class ClassifiedBusiness
    {
        public ClassifiedBusiness()
        {
            BusinessContents = new HashSet<BusinessContent>();
            businessWorkingHours = new HashSet<BusinessWorkingHours>();
        }
        public long ClassifiedBusinessId { get; set; }
        public DateTime? PublishDate { get; set; } 
        public string? UseId { get; set; }
		public string Location { get; set; }
		public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Logo { get; set; }
        public string? phone { get; set; }
        public string? Title { get; set; }
        public string? Mainpic { get; set; }
        public string? Description { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
		public string Reel { get; set; }
		public double Rating { get; set; }
        public int? Views { get; set; }
        public bool IsActive { get; set; }
        //public int? BusinessSubCategoryId { get; set; }
        public double Deliverycost { get; set; }
        public int? BusinessCategoryId { get; set; }
       
        //public int? CountryId { get; set; }
        //public int? CityId { get; set; }
        //public virtual Country Country { get; set; }
        [JsonIgnore]
        public virtual BusinessCategory BusinessCategory { get; set; }
        //public virtual BusinessSubCategory BusinessSubCategory { get; set; }

        //public virtual City City { get; set; }

        public ICollection<Review> Reviews { get; set; }
        public virtual ICollection<BusinessContent> BusinessContents { get; set; }
        public virtual ICollection<BusinessWorkingHours> businessWorkingHours { get; set; }
        public virtual ICollection<BusiniessSubscription>BusiniessSubscriptions { get; set; }
        public virtual ICollection<BDOffer>BDOffers { get; set; }
        public virtual ICollection<BDImage> BDImages { get; set; }

    }
}
