using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class AddListing
    {
        public int AddListingId { get; set; }
        public string CreatedByUser { get; set; }
        public virtual Category Category { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Reequired")]
        public string Title { get; set; }
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string MainLocataion { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string Address { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string ContactPeroson { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string Phone1 { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string Phone2 { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string Fax { get; set; }
        [Required(ErrorMessage ="Required"), RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Not Valid")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string Website { get; set; }
        [Required(ErrorMessage = "Reequired")]
        public string Discription { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string Tags { get; set; }
        public double Rating { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string ListingBanner { get; set; }
        //[Required(ErrorMessage = "Reequired")]
        public string ListingLogo { get; set; }
        public string PromoVideo { get; set; }
        public DateTime? AddedDate { get; set; }
        public ICollection<Branch> Branches { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ListingPhotos> ListingPhotos { get; set; }
        public ICollection<ListingVideos> ListingVideos { get; set; }
    }
}
