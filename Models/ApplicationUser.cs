using Microsoft.AspNetCore.Identity;
using Vision.Models;
namespace Vision
{
    #nullable disable
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string Profilebanner { get; set; }
        public string Nationality { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Bio { get; set; }
        public string Qualification { get; set; }
        public string Job { get; set; }
        public string Gender { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string LinkedInLink { get; set; }
        public string YoutubeLink { get; set; }
        public string FacebookLink { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string MapLocation { get; set; }
        public string NickName { get; set; }
        public string Phone2 { get; set; }
        public string MaritalStatus { get; set; }
        public string Website { get; set; }
        public int Folwers { get; set; }
        public int AvailableClassified { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Interest> Interests { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<LifeEvent> LifeEvents { get; set; }
        //public int AvailableClassified = 0;

    }
}