using Vision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Models
{
    public class UserProfile
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
      
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Bio { get; set; }
        
        public string Qualification { get; set; }
        public string Job { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string Profilebanner { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }
        public string LinkedInLink { get; set; }
        public string YoutubeLink { get; set; }
        public string NickName { get; set; }
        public string Phone2 { get; set; }
        public string MaritalStatus { get; set; }
        public string Website { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string MapLocation { get; set; }
        public int Folwers { get; set; }
        public List<Video> Videos { get; set; }
        public List<Education> Educations { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Interest> Interests { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Language> Languages { get; set; }
        public List<LifeEvent> LifeEvents { get; set; }
    }
}
