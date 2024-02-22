using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using NToastNotify;

namespace Vision.Pages
{
    public class AddClassifiedAdsModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification { get; }
        public HttpClient httpClient { get; set; }
        private static long ClassifiedId = 1;
        public AddClassifiedAdsModel(CRMDBContext Context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _context = Context;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }
        public async Task<ActionResult> OnGet()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Redirect("/identity/account/login");

            //}
            return Page();
        }
        public IActionResult OnGetInfoList(int CatId)
        {
            var Result = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == CatId).OrderBy(e => e.SortOrder).Select(c => new
            {
                ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                AdTemplateFieldCaptionAr = c.AdTemplateFieldCaptionAr,
                AdTemplateFieldCaptionEn = c.AdTemplateFieldCaptionEn,
                FieldTypeId = c.FieldTypeId,
                AdTemplateConfigId = c.AdTemplateConfigId,
                IsRequired = c.IsRequired,
                ValidationMessageAr = c.ValidationMessageAr,
                ValidationMessageEn = c.ValidationMessageEn,
                AdTemplateOptions = c.AdTemplateOptions
            }).ToList();


            return new JsonResult(Result);
        }
        public async Task<ActionResult> OnPostAddClassifiedAdMedia(IFormCollection MyUploader)
        {
            try
            {
                if (MyUploader != null)
                {
                    if (MyUploader.Files.Count > 0)
                    {
                        foreach (var elem in MyUploader.Files)
                        {
                            int TemplateConfigId = 0;
                            bool CheckConvert = int.TryParse(elem.FileName, out TemplateConfigId);
                            if (CheckConvert || ClassifiedId == 0)
                            {
                                var contentListMediaValue = new List<AdContentValue>();
                                string folder = "Images/ClassifiedAds/";
                                var contentObj = new AdContentValue();
                                contentObj.ContentValue = UploadImage(folder, elem);
                                contentListMediaValue.Add(contentObj);
                                var ContentValueObj = new AdContent()
                                {
                                    ClassifiedAdId = ClassifiedId,
                                    AdTemplateConfigId = TemplateConfigId,
                                    AdContentValues = contentListMediaValue

                                };
                                _context.AdContents.Add(ContentValueObj);
                                _context.SaveChanges();

                            }

                        }
                    }
                }
                _toastNotification.AddSuccessToastMessage("Classified Ads Added Successfully");

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error...Please Try Again");
                return new JsonResult(false);
            }

            return new JsonResult(true);

        }
        public async Task<ActionResult> OnPostAddNewClassifiedAd([FromBody] NewClassifiedAdsVM classifiedAdsVM)
        {

            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _toastNotification.AddSuccessToastMessage("Please Login First");
                    return new JsonResult(false);
                }
                var ClassifiedAdCat = _context.ClassifiedAdsCategories.Find(classifiedAdsVM.ClassifiedAdsCategoryId);
                if (ClassifiedAdCat == null)
                {
                    _toastNotification.AddSuccessToastMessage("Category Not Found");

                }
                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    IsActive = classifiedAdsVM.IsActive,
                    ClassifiedAdsCategoryId = classifiedAdsVM.ClassifiedAdsCategoryId,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,

                };
                _context.ClassifiedAds.Add(classifiedAd);
                _context.SaveChanges();
                ClassifiedId = classifiedAd.ClassifiedAdId;
                foreach (var item in classifiedAdsVM.addContentVMs)
                {
                    var contentList = new List<AdContentValue>();

                    foreach (var elem in item.Values)
                    {

                        var contentObj = new AdContentValue()
                        {
                            ContentValue = elem,
                        };
                        contentList.Add(contentObj);
                    }


                    var ContentValue = new AdContent()
                    {
                        ClassifiedAdId = classifiedAd.ClassifiedAdId,
                        AdTemplateConfigId = item.AdTemplateConfigId,
                        AdContentValues = contentList

                    };
                    _context.AdContents.Add(ContentValue);
                    _context.SaveChanges();
                }

            }

            catch (Exception)
            {
                _toastNotification.AddSuccessToastMessage("Something Went Error...Please Try Again");
                return new JsonResult(false);
            }
            return new JsonResult(true);

        }
        private string UploadImage(string folderPath, IFormFile file)
        {
            string[] firstNames = file.ContentType.Split("/", StringSplitOptions.None);
            folderPath += Guid.NewGuid().ToString() + "_" + firstNames[0]+"." + firstNames[1];

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }


    }
}
