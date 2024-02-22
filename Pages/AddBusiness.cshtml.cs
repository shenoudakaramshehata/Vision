using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using NToastNotify;

namespace Vision.Pages
{
    public class AddBusinessModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification { get; }
        public HttpClient httpClient { get; set; }
        private static long BusinessClassifiedId = 1;
        public AddBusinessModel(CRMDBContext Context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
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
            var Result = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == CatId).OrderBy(e => e.SortOrder).Select(c => new
            {
                BusinessCategoryId = c.BusinessCategoryId,
                BusinessTemplateFieldCaptionAr = c.BusinessTemplateFieldCaptionAr,
                BusinessTemplateFieldCaptionEn = c.BusinessTemplateFieldCaptionEn,
                FieldTypeId = c.FieldTypeId,
                BusinessTemplateConfigId = c.BusinessTemplateConfigId,
                IsRequired = c.IsRequired,
                ValidationMessageAr = c.ValidationMessageAr,
                ValidationMessageEn = c.ValidationMessageEn,
                BusinessTemplateOptions = c.BusinessTemplateOptions
            }).ToList();


            return new JsonResult(Result);
        }
        public async Task<ActionResult> OnPostAddBusinessMedia(IFormCollection MyUploader)
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
                            if (CheckConvert || BusinessClassifiedId == 0)
                            {
                                var contentListMediaValue = new List<BusinessContentValue>();
                                string folder = "Images/Business/";
                                var contentObj = new BusinessContentValue();
                                contentObj.ContentValue = UploadImage(folder, elem);
                                contentListMediaValue.Add(contentObj);
                                var ContentValueObj = new BusinessContent()
                                {
                                    ClassifiedBusinessId = BusinessClassifiedId,
                                    BusinessTemplateConfigId = TemplateConfigId,
                                    BusinessContentValues = contentListMediaValue

                                };
                                _context.BusinessContents.Add(ContentValueObj);
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
        public async Task<ActionResult> OnPostAddNewBusiness([FromBody] NewClassifiedBusinessVM classifiedBusinessVM)
        {

            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _toastNotification.AddErrorToastMessage("Please Login First");
                    return new JsonResult(false);
                }
                var ClassifiedBusinessCat = _context.BusinessCategories.Find(classifiedBusinessVM.BusinessCategoryId);
                if (ClassifiedBusinessCat == null)
                {
                    _toastNotification.AddErrorToastMessage("Category Not Found");

                }
                ClassifiedBusiness classifiedBusiness = new ClassifiedBusiness()
                {
                    IsActive = classifiedBusinessVM.IsActive,
                    BusinessCategoryId = classifiedBusinessVM.BusinessCategoryId,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,

                };
                _context.ClassifiedBusiness.Add(classifiedBusiness);
                _context.SaveChanges();
                BusinessClassifiedId = classifiedBusiness.ClassifiedBusinessId;
                foreach (var item in classifiedBusinessVM.addContentVMs)
                {
                    var contentList = new List<BusinessContentValue>();

                    foreach (var elem in item.Values)
                    {

                        var contentObj = new BusinessContentValue()
                        {
                            ContentValue = elem,
                        };
                        contentList.Add(contentObj);
                    }


                    var ContentValue = new BusinessContent()
                    {
                        ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
                        BusinessTemplateConfigId = item.BusinessTemplateConfigId,
                        BusinessContentValues = contentList

                    };
                    _context.BusinessContents.Add(ContentValue);
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
            folderPath += Guid.NewGuid().ToString() + "_" + firstNames[0] + "." + firstNames[1];

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
