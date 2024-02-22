using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;
using Vision.Data;
using Vision.ViewModels;

namespace Vision.Areas.CRM.Pages.Configurations.Users
{
    public class ProfileModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        public List<Skill> skills { get; set; }
        public List<Language> languages { get; set; }
        public List<Photo> photos { get; set; }

        public List<Interest> interests { get; set; }
        public List<Education> education { get; set; }
        public List<LifeEvent> lifeevent { get; set; }

        
        [BindProperty]
        public UserProfileMainInfoVM userProfileMainIfoVM { get; set; }
        public ProfileModel(CRMDBContext context, ApplicationDbContext db, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            userProfileMainIfoVM = new UserProfileMainInfoVM();
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            skills = new List<Skill>();
            languages = new List<Language>();
            photos = new List<Photo>();
            interests = new List<Interest>();
            education = new List<Education>();
            lifeevent= new List<LifeEvent>();
        }
        public async Task<IActionResult> OnGet(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);

                userProfileMainIfoVM.Phone = user.PhoneNumber;
                userProfileMainIfoVM.FullName = user.FullName;
                userProfileMainIfoVM.Email = user.Email;
                userProfileMainIfoVM.Image = user.ProfilePicture;
                userProfileMainIfoVM.Followers = user.Folwers;
                userProfileMainIfoVM.birthDay = user.BirthDate ;
                userProfileMainIfoVM.Gender = user.Gender;
                userProfileMainIfoVM.Job = user.Job;
                userProfileMainIfoVM.Location = user.MapLocation;
                userProfileMainIfoVM.Nationality = user.Nationality;
                userProfileMainIfoVM.nickName = user.NickName;
                userProfileMainIfoVM.Qualification = user.Qualification;
                userProfileMainIfoVM.martialStatus = user.MaritalStatus;
                userProfileMainIfoVM.instagram = user.InstagramLink;
                userProfileMainIfoVM.Twitter = user.TwitterLink;
                userProfileMainIfoVM.Linkdin = user.LinkedInLink;
                userProfileMainIfoVM.webSite = user.Website;
                userProfileMainIfoVM.Bio = user.Bio;
                skills = _db.Skills.Where(e => e.Id == user.Id).ToList();
                languages = _db.Languages.Where(e => e.Id == user.Id).ToList();
                photos = _db.Photos.Where(e => e.Id == user.Id).ToList();
                interests= _db.Interests.Where(e => e.Id == user.Id).ToList();
                education= _db.Educations.Where(e => e.Id == user.Id).ToList();
                lifeevent= _db.LifeEvents.Where(e => e.Id == user.Id).ToList();
            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

            }

            return Page();
        }
    }
}
