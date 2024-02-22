using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace Vision.Areas.TemplatePages.Pages
{
#nullable disable
    public class PostAdsModel : PageModel
    {

        private readonly CRMDBContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPartialToStringRenderer _renderer;
        private List<AdContentValue> contentList = new List<AdContentValue>();
        public List<AdTemplateConfig> lstColumns { get; set; }
        public List<City> Cities =new List<City>();
        public List<Area> Areas =new List<Area>();
        private readonly IWebHostEnvironment _hostEnvironment;
        private ApplicationDbContext _applicationDbContext { get; set; }

        public HttpClient httpClient { get; set; }
        public string FormTemplate { get; set; }
        public static int AdCategoryId = 0;
        [BindProperty]
        public AddClassifiedAdVm ClassifiedAdObj { get; set; }
        public PostAdsModel(IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, CRMDBContext Context, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _context = Context;
            _userManager = userManager;
            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _applicationDbContext = applicationDbContext;

        }

        public async Task<IActionResult> OnGet(int categoryId)
        {
            try
            {

                var ClassifiedCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == categoryId).FirstOrDefault();
                if (ClassifiedCategory == null)
                {
                    return Redirect("/PageNF");
                }
                AdCategoryId = ClassifiedCategory.ClassifiedAdsCategoryId;
                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale.RequestCulture.UICulture.ToString();

                
                if (ClassifiedCategory != null)
                {
                    lstColumns = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == categoryId).OrderBy(c => c.SortOrder).ToList();
                }

                bool isFound = System.IO.File.Exists(_hostEnvironment.WebRootPath + $"\\AdsTemplates\\{categoryId}.txt");
                if (isFound == false)
                {
                    return Redirect("/PageNF");
                }
                Cities = _context.Cities.ToList();
                FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + $"\\AdsTemplates\\{categoryId}.txt");
                foreach (var col in lstColumns)
                {
                    switch (col.FieldType.FieldTypeTitle)
                    {
                        case "Text":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayText", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARText", col));

                            }
                            break;
                        case "Number":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayNumber", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARNumber", col));

                            }
                          
                            break;
                        case "Lookup":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayLookup", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARLookup", col));

                            }
                            
                            break;
                        case "DateTime":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayDateTime", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARDateTime", col));

                            }                            
                            break;

                        case "CheckBox":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayCheckBox", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARCheckBox", col));

                            }                            
                            break;

                        case "CheckBoxGroup":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayCheckBoxGroup", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARCheckBoxGroup", col));

                            }
                            
                            break;

                        case "LargeText":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayLargeText", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayArLargeText", col));

                            }                            
                            break;
                        case "Image":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayImage", col));
                            break;
                        case "Images":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayImages", col));
                            break;
                        case "Video":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayVideo", col));
                            break;

                        case "Videos":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayVideos", col));
                            break;
                        //case "Media":
                        //    Html.Partial("~/Pages/Shared/Components/InputForm/ComDisplayMedia.cshtml", col);
                        //    break;
                        case "RadioGroup":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayRadioGroup", col));
                            break;
                        case "Map (Location)":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayMap", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayARMap", col));

                            }
                            
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return Page();
        }

        public async Task<ActionResult> OnPost(IFormFile pic, IFormFileCollection Photos,int SelectedArea)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }

                //Loop columns and get form object by id
                var fields = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == 59).OrderBy(c => c.SortOrder).ToList();


                //Add classified Ad
                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    TitleAr = ClassifiedAdObj.TitleAr,
                    TitleEn = ClassifiedAdObj.TitleEn,
                    Description = ClassifiedAdObj.Description,
                    IsActive = ClassifiedAdObj.IsActive,
                    Price = ClassifiedAdObj.Price,
                    CityId = ClassifiedAdObj.CityId,
                    AreaId = SelectedArea,
                    ClassifiedAdsCategoryId = AdCategoryId,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    Views = 0
                };
                if (pic != null)
                {
                    classifiedAd.MainPic = UploadImage("Images/ClassifiedAds/", pic);


                }
                if (Photos != null)
                {
                    List<AdsImage> AdSlider = new List<AdsImage>();
                    foreach (var item in Photos)
                    {
                        var AdsImageObj = new AdsImage();
                        AdsImageObj.Image = UploadImage("Images/ClassifiedAds/", item);
                        AdSlider.Add(AdsImageObj);

                    }
                    classifiedAd.AdsImages = AdSlider;

                }
                _context.ClassifiedAds.Add(classifiedAd);
                _context.SaveChanges();
                user.AvailableClassified = user.AvailableClassified - 1;
                _applicationDbContext.SaveChanges();
                long ClassifiedAdId = classifiedAd.ClassifiedAdId;
                //Adding Ads Content and Content Options
                foreach (AdTemplateConfig field in fields)
                {

                    int ConfigId = field.AdTemplateConfigId;

                    var Inputvalue = Request.Form[ConfigId.ToString()];

                    Console.WriteLine("Field:" + field.AdTemplateFieldCaptionEn + "----" + "User Input is:" + Inputvalue.ToString());

                    switch (field.FieldTypeId)
                    {
                        case 1:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 2:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 3:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 4:
                            string DatetimeN = "datepicker-here";
                             Inputvalue = Request.Form[DatetimeN];
                           

                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 5:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 6:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 7:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 8:
                            string fileN = ConfigId.ToString();
                            IFormFile file = Request.Form.Files[fileN];
                            addSingleFile(file, ConfigId, ClassifiedAdId);
                            break;
                        case 9:
                            //string imagesConfig = "Upload " + ConfigId.ToString();
                            string imagesConfig = ConfigId.ToString();

                            List<IFormFile> imagesFiles = new List<IFormFile>();
                            for (int i = 0; i < Response.HttpContext.Request.Form.Files.Count(); i++)
                            {
                                if (Response.HttpContext.Request.Form.Files[i].Name == imagesConfig)
                                {
                                    imagesFiles.Add(Response.HttpContext.Request.Form.Files[i]);
                                };

                            }
                            addMultiFiles(imagesFiles, ConfigId, ClassifiedAdId);
                            break;
                        case 10:
                            string configVedio = ConfigId.ToString();
                            IFormFile filevedio = Request.Form.Files[configVedio];
                            addSingleFile(filevedio, ConfigId, ClassifiedAdId);
                            break;
                        //case 11:
                        //    string videosConfig = ConfigId.ToString();
                        //    List<IFormFile> videosFiles = new List<IFormFile>();
                        //    for (int i = 0; i < Response.HttpContext.Request.Form.Files.Count(); i++)
                        //    {
                        //        if (Response.HttpContext.Request.Form.Files[i].Name == videosConfig)
                        //        {
                        //            videosFiles.Add(Response.HttpContext.Request.Form.Files[i]);
                        //        };

                        //    }
                        //    addMultiFiles(videosFiles, ConfigId, ClassifiedAdId);
                        //    break;
                        case 13:
                            addContentValue(Inputvalue, ConfigId, ClassifiedAdId);
                            break;
                        case 14:

                            string Mapvalue = Request.Form["LatId"];
                            addMapValue(Mapvalue, ConfigId, ClassifiedAdId);
                            break;

                        default:
                            break;
                    }


                }
                _context.SaveChanges();
                return Redirect("/Ads");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void addContentValue(string value, int AdTemplateConfigId, long ClassifiedAdId)
        {

            List<AdContentValue> adContentValues = new List<AdContentValue>();
            if (value != null)
            {
                var values = value.Split(",");
                
                for (int i = 0; i < values.Length; i++)
                {
                    var contentObj = new AdContentValue()
                    {
                        ContentValue = values[i],
                        
                    };
                    adContentValues.Add(contentObj);
                }
                AdContent adContent = new AdContent()
                {
                    ClassifiedAdId = ClassifiedAdId,
                    AdTemplateConfigId = AdTemplateConfigId,
                    AdContentValues = adContentValues
                };
                _context.AdContents.Add(adContent);


            }

        }
        public void addMapValue(string value, int AdTemplateConfigId, long ClassifiedAdId)
        {

            List<AdContentValue> adContentValues = new List<AdContentValue>();
            var contentObj = new AdContentValue()
            {
                ContentValue = value,
            };
            adContentValues.Add(contentObj);
            AdContent adContent = new AdContent()
            {
                ClassifiedAdId = ClassifiedAdId,
                AdTemplateConfigId = AdTemplateConfigId,
                AdContentValues = adContentValues
            };
            _context.AdContents.Add(adContent);


        }

		public void addMultiFiles(List<IFormFile> files, int AdTemplateConfigId, long ClassifiedAdId)
		{
			List<AdContentValue> adContentValues = new List<AdContentValue>();
			foreach (var item in files)
			{
				if (item != null)
				{
					string folder = "Images/ClassifiedAds/";
					var contentObj = new AdContentValue();
					contentObj.ContentValue = UploadImage(folder, item);
					adContentValues.Add(contentObj);
				}

			}


			var AdContentObj = new AdContent()
			{
				ClassifiedAdId = ClassifiedAdId,
				AdTemplateConfigId = AdTemplateConfigId,
				AdContentValues = adContentValues

			};
			_context.AdContents.Add(AdContentObj);

		}
		public void addSingleFile(IFormFile file, int AdTemplateConfigId, long ClassifiedAdId)
        {
            List<AdContentValue> adContentValues = new List<AdContentValue>();

            if (file != null)
            {
                string folder = "Images/ClassifiedAds/";
                var contentObj = new AdContentValue();
                contentObj.ContentValue = UploadImage(folder, file);
                adContentValues.Add(contentObj);
            }


            var ContentValueObj = new AdContent()
            {
                ClassifiedAdId = ClassifiedAdId,
                AdTemplateConfigId = AdTemplateConfigId,
                AdContentValues = adContentValues

            };
            _context.AdContents.Add(ContentValueObj);

        }
        public async Task<IActionResult> OnGetSingleFillData(int CityId)
        {
            var Result = _context.Areas.Where(e => e.CityId == CityId).ToList();
            return new JsonResult(Result);
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
