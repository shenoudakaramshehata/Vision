using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public ApplicationUser userProfile { get; set; }
        public CRMDBContext _context { get; set; }
        public IToastNotification ToastNotification { get; }
        public ApplicationDbContext _applicationDbContext { get; set; }
        public static List<Skill> Skills;
        public List<Skill> SkillsList = new List<Skill>();
        public static List<Interest> Interests;
        public List<Interest> InterestsList = new List<Interest>();
        public static List<Language> Languages;
        public List<Language> LanguagesList = new List<Language>();
        //[BindProperty]
        public Education education { get; set; }
        public static List<Education> Educations;
        public List<Education> EducationsList = new List<Education>();
        public LifeEvent LifeEvent { get; set; }
        public static ApplicationUser user { set; get; }
        public static List<LifeEvent> LifeEvents;
        public List<LifeEvent> LifeEventsList = new List<LifeEvent>();
        public List<Video> userVideos = new List<Video>();
        public List<Photo> userPhotos = new List<Photo>();
        [ViewData]
        public string educationvalidation { get; set; }
        public ProfileModel(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, CRMDBContext Context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
            ToastNotification = toastNotification;
            _context = Context;
            _applicationDbContext = applicationDbContext;
            userProfile = new ApplicationUser();

        }
        public IActionResult OnPostFillSkillsList([FromBody] List<string> SkillsTitle)
        {
            var Skillsid = _applicationDbContext.Skills.Where(a => a.Id == user.Id);
            _applicationDbContext.Skills.RemoveRange(Skillsid);
            _applicationDbContext.SaveChanges();
            Skills = new List<Skill>();
            foreach (var item in SkillsTitle)
            {
                Skill Skill = new Skill { SkillTitle = item };
                Skills.Add(Skill);
            }
            ToastNotification.AddSuccessToastMessage("Skill..");

            return new JsonResult(Skills);
        }
        public IActionResult OnPostFillInterestsList([FromBody] List<string> InterestsTitle)
        {
            var Interristid = _applicationDbContext.Interests.Where(a => a.Id == user.Id);
            _applicationDbContext.Interests.RemoveRange(Interristid);
            _applicationDbContext.SaveChanges();
            Interests = new List<Interest>();
            foreach (var item in InterestsTitle)
            {
                Interest Interrist = new Interest { InterestTitle = item };
                Interests.Add(Interrist);
            }
            return new JsonResult(Interests);
        }

        public IActionResult OnPostFillLanguagesList([FromBody] List<string> LanguagesTitle)
        {
            var Languagesid = _applicationDbContext.Languages.Where(a => a.Id == user.Id);
            _applicationDbContext.Languages.RemoveRange(Languagesid);
            _applicationDbContext.SaveChanges();

            Languages = new List<Language>();
            foreach (var item in LanguagesTitle)
            {

                Language language = new Language { LanguageTitle = item };
                Languages.Add(language);
            }
            return new JsonResult(Languages);
        }

        public IActionResult OnPostFillEducationsList([FromBody] List<Education> Educationslist)
        {
            if (Educationslist == null)
            {
                return Page();
            }
            var Educationsid = _applicationDbContext.Educations.Where(a => a.Id == user.Id);
            _applicationDbContext.Educations.RemoveRange(Educationsid);
            _applicationDbContext.SaveChanges();
            foreach (var item in Educationslist)
            {
                if (item.Provider == "")
                {
                    educationvalidation = "Is Required";
                }
            }
            Educations = Educationslist;
            return new JsonResult(Educations);
        }
        public IActionResult OnPostFillLifeEventsList([FromBody] List<LifeEvent> lifeEventsList)
        {
            if (lifeEventsList == null)
            {
                return Page();
            }
            var eventlist = _applicationDbContext.LifeEvents.ToList();
            _applicationDbContext.LifeEvents.RemoveRange(eventlist);
            int counter = lifeEventsList.Count();
            List<LifeEvent> NewlifeEventsList = new List<LifeEvent>();
            for (int i = 0; i < counter; i++)
            {
                if (lifeEventsList.Count()!=0)
                {
                   
                    if (lifeEventsList[i].Caption == "" && lifeEventsList[i].Details == "" && lifeEventsList[i].EventType == "")
                    {
                        lifeEventsList.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        LifeEvent lifeEventsObj = new LifeEvent()
                        {
                            Caption = lifeEventsList[i].Caption,
                            Details = lifeEventsList[i].Details,
                            EventType = lifeEventsList[i].EventType,
                            Id = user.Id,
                            Media = lifeEventsList[i].Media
                        };
                        NewlifeEventsList.Add(lifeEventsObj);
                    }
                }
            }

            _applicationDbContext.LifeEvents.AddRange(NewlifeEventsList);
            _applicationDbContext.SaveChanges();

            LifeEvents = NewlifeEventsList;
            return new JsonResult(NewlifeEventsList);
        }
        public IActionResult OnPostDeletePhotoById([FromBody] int PhotoId)
        {
            try
            {
                var photoobj = _applicationDbContext.Photos.Where(a => a.PhotoID == PhotoId).FirstOrDefault();
                if (photoobj == null)
                {
                    return Page();
                }
                _applicationDbContext.Photos.Remove(photoobj);
                _applicationDbContext.SaveChanges();
                return new JsonResult(PhotoId);
            }
            catch (Exception)
            {
                ToastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
        }
        public IActionResult OnPostDeleteVideoById([FromBody] int Videoid)
        {
            try
            {
                var Videoobj = _applicationDbContext.Videos.Where(a => a.VideoID == Videoid).FirstOrDefault();
                if (Videoobj == null)
                {
                    return Page();
                }
                _applicationDbContext.Videos.Remove(Videoobj);
                _applicationDbContext.SaveChanges();
                return new JsonResult(Videoid);
            }
            catch (Exception)
            {
                ToastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
        }



        public IActionResult OnPostDeleteMediaById([FromBody] int Mediaid)
        {
            try
            {
                var Videoobj = _applicationDbContext.LifeEvents.Where(a => a.LifeEventID == Mediaid).FirstOrDefault();
                if (Videoobj == null)
                {
                    return Page();
                }
                //Videoobj.Media = null;
                _applicationDbContext.SaveChanges();
                LifeEventsList = LifeEvents = _applicationDbContext.LifeEvents.Where(a => a.Id == user.Id).ToList();

                return new JsonResult(Mediaid);
            }
            catch (Exception)
            {
                ToastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
        }
        public async Task<IActionResult> OnGet()
        {
            //user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Redirect("/identity/account/login");
            //}
            //userProfile = user;
            //SkillsList = Skills = _applicationDbContext.Skills.Where(a => a.Id == user.Id).ToList();
            //InterestsList = Interests = _applicationDbContext.Interests.Where(a => a.Id == user.Id).ToList();
            //LanguagesList = Languages = _applicationDbContext.Languages.Where(a => a.Id == user.Id).ToList();
            //EducationsList = Educations = _applicationDbContext.Educations.Where(a => a.Id == user.Id).ToList();
            //LifeEventsList = LifeEvents = _applicationDbContext.LifeEvents.Where(a => a.Id == user.Id).ToList();
            //userPhotos = _applicationDbContext.Photos.Where(a => a.Id == user.Id).ToList();
            //userVideos = _applicationDbContext.Videos.Where(a => a.Id == user.Id).ToList();
            return Page();
        }


        public async Task<IActionResult> OnPost(IFormFile Profilepic, IFormFile bannerpic, IFormFileCollection Images, IFormFileCollection VideosFiels, List<IFormFile> LifeeventMedia)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                user.Gender = userProfile.Gender;
                user.Bio = userProfile.Bio;
                user.Nationality = userProfile.Nationality;
                user.BirthDate = userProfile.BirthDate;
                user.Job = userProfile.Job;
                user.Qualification = userProfile.Qualification;
                user.FullName = userProfile.FullName;
                user.PhoneNumber = userProfile.PhoneNumber;
                user.FacebookLink = userProfile.FacebookLink;
                user.TwitterLink = userProfile.TwitterLink;
                user.InstagramLink = userProfile.InstagramLink;
                user.LinkedInLink = userProfile.LinkedInLink;
                user.YoutubeLink = userProfile.YoutubeLink;
                user.NickName = userProfile.NickName;
                user.MaritalStatus = userProfile.MaritalStatus;
                user.City = userProfile.City;
                user.MapLocation = userProfile.MapLocation;
                user.Country = userProfile.Country;
                user.Phone2 = userProfile.Phone2;
                user.Folwers = userProfile.Folwers;
                user.Website = userProfile.Website;
                user.Skills = Skills == null ? new List<Skill>() : Skills;
                user.Interests = Interests == null ? new List<Interest>() : Interests;
                user.Educations = Educations == null ? new List<Education>() : Educations;
                user.Languages = Languages == null ? new List<Language>() : Languages;

                //if (LifeEvents.Count() == LifeeventMedia.Count())
                //{
                int n = 0;
                for (int i = 0; i < LifeEvents.Count(); i++)
                {
                    if (LifeEvents[i].Media == "")
                    {
                        string folder = "Images/ProfileImages/";
                        LifeEvents[i].Media = UploadImage(folder, LifeeventMedia[n]);
                        n++;
                    }

                }
                // _applicationDbContext.LifeEvents.RemoveRange(user.LifeEvents);

                user.LifeEvents = LifeEvents;
                //}
                if (Images.Count() > 0)
                {
                    List<Photo> userpho = new List<Photo>();
                    for (int i = 0; i < Images.Count(); i++)
                    {
                        Photo photo = new Photo();
                        if (Images[i] != null)
                        {
                            string folder = "Images/ProfileImages/";
                            photo.Image = UploadImage(folder, Images[i]);
                            photo.PublishDate = DateTime.Now;
                            photo.Caption = "Photo";
                            photo.Id = user.Id;
                        }
                        userpho.Add(photo);
                    }
                    user.Photos = userpho;
                }
                if (VideosFiels.Count() > 0)
                {
                    List<Video> uservid = new List<Video>();
                    for (int i = 0; i < VideosFiels.Count(); i++)
                    {
                        Video video = new Video();
                        if (VideosFiels[i] != null)
                        {
                            string folder = "Images/ProfileVideos/";
                            video.VideoT = UploadImage(folder, VideosFiels[i]);
                            video.Caption = "Videio";
                            video.PublishDate = DateTime.Now;
                            video.Id = user.Id;
                        }
                        uservid.Add(video);
                    }
                    user.Videos = uservid;
                }
                _applicationDbContext.SaveChanges();

                if (Profilepic != null)
                {
                    string folder = "Images/ProfileImages/";
                    user.ProfilePicture = UploadImage(folder, Profilepic);
                }
                if (bannerpic != null)
                {
                    string folder = "Images/BannerImages/";
                    user.Profilebanner = UploadImage(folder, bannerpic);
                }
                await _userManager.UpdateAsync(user);
                ToastNotification.AddSuccessToastMessage("Profile Updated Successfully..");
                return RedirectToPage("Profile");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }


    }

}
