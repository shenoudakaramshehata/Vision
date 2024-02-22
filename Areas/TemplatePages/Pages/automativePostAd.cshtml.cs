using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using Vision.Data;
using Vision.Models;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Vision.Areas.TemplatePages.Pages
{
    public class automativePostAdModel : PageModel
    {
        private readonly CRMDBContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPartialToStringRenderer _renderer;
        private List<AdContentValue> contentList = new List<AdContentValue>();
        public List<AdTemplateConfig> lstColumns { get; set; }
        private readonly IWebHostEnvironment _hostEnvironment;

        public HttpClient httpClient { get; set; }
        public string FormTemplate { get; set; }

        public automativePostAdModel(IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, CRMDBContext Context, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;

        }

        public async void OnGet()
        {
            try
            {

                var ClassifiedCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == 1).FirstOrDefault();

                if (ClassifiedCategory != null)
                {
                    lstColumns = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == 2).OrderBy(c => c.SortOrder).ToList();
                }

                FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + "\\AdsTemplates\\1.txt");
                foreach (var col in lstColumns)
                {
                    switch (col.FieldType.FieldTypeTitle)
                    {
                        case "Text":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayText", col));
                            break;
                        case "Number":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayNumber", col));

                            break;
                        
                            
                        case "Lookup":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayLookup", col));
                            break;
                        case "DateTime":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayDateTime", col));
                            break;
                        case "CheckBox":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayCheckBox", col));
                            break;
                        case "CheckBoxGroup":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayCheckBoxGroup", col));
                            break;
                        case "LargeText":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayLargeText", col));
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
                        
                        case "RadioGroup":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayRadioGroup", col));
                            break;
                        case "Map (Location)":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComDisplayMap", col));
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
        }

        public async Task<ActionResult> OnPost()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }

                //Loop columns and get form object by id
                var fields = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == 1).OrderBy(c => c.SortOrder).ToList();


                //Add classified Ad
                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    IsActive = true,
                    ClassifiedAdsCategoryId = 1,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    Views = 0
                };

                _context.ClassifiedAds.Add(classifiedAd);
                _context.SaveChanges();
                long ClassifiedAdId = classifiedAd.ClassifiedAdId;
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
                        case 11:
                            string videosConfig = ConfigId.ToString();
                            List<IFormFile> videosFiles = new List<IFormFile>();
                            for (int i = 0; i < Response.HttpContext.Request.Form.Files.Count(); i++)
                            {
                                if (Response.HttpContext.Request.Form.Files[i].Name == videosConfig)
                                {
                                    videosFiles.Add(Response.HttpContext.Request.Form.Files[i]);
                                };

                            }
                            addMultiFiles(videosFiles, ConfigId, ClassifiedAdId);
                            break;
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
                int count = contentList.Count();
                string g = "";


                return default;

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
                _context.SaveChanges();

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
            _context.SaveChanges();

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
            _context.SaveChanges();
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
            _context.SaveChanges();
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
