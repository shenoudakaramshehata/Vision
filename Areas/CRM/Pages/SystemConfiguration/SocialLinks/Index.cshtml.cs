
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision.Areas.CRM.Pages.SystemConfiguration.SocialLinks
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public SoicialMidiaLink socialMediaLink { get; set; }


        public List<SoicialMidiaLink> socialMediaLinksList = new List<SoicialMidiaLink>();
        
        public SoicialMidiaLink socialMediaObj { get; set; }

        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, 
                                            IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            socialMediaLink = new SoicialMidiaLink();
            socialMediaObj = new SoicialMidiaLink();
        }
        public void OnGet()
        {
            socialMediaLinksList = _context.SoicialMidiaLinks.ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
        }

        public IActionResult OnGetSingleSocialForEdit(int SocialMediaLinkId)
        {
            socialMediaLink = _context.SoicialMidiaLinks.Where(c => c.id == SocialMediaLinkId).FirstOrDefault();
            
            return new JsonResult(socialMediaLink);
        }

        public IActionResult OnPostEditSocial(int id)
        {
            try
            {
                var model = _context.SoicialMidiaLinks
                                    .Where(c => c.id == id)
                                    .FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("SocialObj Not Found");
                 
                    return Redirect("/CRM/SystemConfiguration/SocialLinks/Index");
                }


                model.facebooklink = socialMediaLink.facebooklink;
                model.TwitterLink = socialMediaLink.TwitterLink;
                model.Instgramlink = socialMediaLink.Instgramlink;
                model.LinkedInlink = socialMediaLink.LinkedInlink;
                model.WhatsApplink = socialMediaLink.WhatsApplink;
                model.YoutubeLink = socialMediaLink.YoutubeLink;
               

                var UpdatedSocialLinks = _context.SoicialMidiaLinks.Attach(model);

                UpdatedSocialLinks.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Links Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
               
            }
            return Redirect("/CRM/SystemConfiguration/SocialLinks/Index");
        }

    }
}
