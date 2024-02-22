using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
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
namespace Vision.Pages
{
    public class postBusinessModel : PageModel
    {
        private readonly CRMDBContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPartialToStringRenderer _renderer;
        private List<AdContentValue> contentList = new List<AdContentValue>();
        public List<AdTemplateConfig> lstColumns { get; set; }
        private readonly IWebHostEnvironment _hostEnvironment;
        public List<BussinessPlan> bussinessPlans { get; set; }
        public HttpClient httpClient { get; set; }
        public string FormTemplate { get; set; }

        public postBusinessModel(IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, CRMDBContext Context, UserManager<ApplicationUser> userManager)
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

                var ClassifiedCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == 48).FirstOrDefault();
                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale.RequestCulture.UICulture.ToString();
                bussinessPlans = _context.BussinessPlans.ToList();

                if (ClassifiedCategory != null)
                {
                    lstColumns = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == 48).OrderBy(c => c.SortOrder).ToList();
                }

                //Read HTMl Template from file
                //FormTemplate = "  <div class=\"dashboard-title   fl-wrap\">\r\n<h3>Add Listing</h3>\r\n</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n " +
                //               "  <div class=\"custom-form\">\r\n<div class=\"row\">\r\n<div class=\"col-md-12\">\r\n##1263##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n" +
                //               "  <div class=\"col-md-12\">\r\n##1269##\r\n</div>\r\n</div>\r\n</div>\r\n</div>\r\n<div class=\"dashboard-title  dt-inbox fl-wrap\">\r\n<h3>Price</h3>\r\n</div>" +
                //               "\r\n<div class=\"profile-edit-container fl-wrap  block_box\">\r\n<div class=\"custom-form add_room-item-wrap\">\r\n" +
                //               "<div class=\"add_room-container fl-wrap\">\r\n<!--add_room-item end  -->\r\n<div class=\"add_room-item fl-wrap\">\r\n<span class=\"remove-rp tolt\" " +
                //               "data-microtip-position=\"left\" data-tooltip=\"Remove\"><i class=\"fal fa-times-circle\"></i></span>\r\n<div class=\"row\">\r\n<div class=\"col-md-4\">\r\n##1265##\r\n</div>\r\n" +
                //               "<div class=\"col-md-4\">\r\n##1264##\r\n</div>\r\n<div class=\"col-md-4\">\r\n##1267##\r\n</div>\r\n</div>\r\n</div>\r\n<!--add_room-item end  -->\r\n</div>\r\n</div>\r\n</div>\r\n" +
                //               "<div class=\"dashboard-title  dt-inbox fl-wrap\">\r\n<h3>Facilities</h3>\r\n</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n" +
                //               "<div class=\"custom-form\">\r\n##1268##\r\n<!-- Checkboxes -->\r\n<!-- Checkboxes end -->\r\n</div>\r\n</div>\r\n<div class=\"dashboard-title  dt-inbox fl-wrap\">\r\n<h3>Photos</h3>\r\n" +
                //               "</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n<div class=\"custom-form\">\r\n<div class=\"row\">\r\n<!--col -->\r\n<div class=\"col-md-4\">\r\n" +
                //               "<div class=\"add-list-media-wrap\">\r\n<div class=\"listsearch-input-item fl-wrap\">\r\n<div class=\"fuzone\">\r\n<form>\r\n<div class=\"fu-text\">\r\n<span>" +
                //               "<i class=\"fal fa-images\"></i> Click here or drop files to upload</span>\r\n<div class=\"photoUpload-files fl-wrap\"></div>\r\n</div>\r\n##1270##\r\n</form>\r\n</div>\r\n</div>\r\n" +
                //               "</div>\r\n</div>\r\n<!--col end-->\r\n<!--col -->\r\n<div class=\"col-md-8\">\r\n<div class=\"add-list-media-wrap\">\r\n<div class=\"listsearch-input-item fl-wrap\">\r\n" +
                //               "<div class=\"fuzone\">\r\n<form>\r\n<div class=\"fu-text\">\r\n<span><i class=\"fal fa-images\"></i> Click here or drop files to upload</span>\r\n<div class=\"photoUpload-files fl-wrap\">" +
                //               "</div>\r\n</div>\r\n##1279##\r\n</form>\r\n</div>\r\n</div>\r\n</div>\r\n</div>\r\n<!--col end-->\r\n</div>\r\n</div>\r\n</div>\r\n<div class=\"dashboard-title  dt-inbox fl-wrap\">" +
                //               "\r\n<h3>Location / Contacts</h3>\r\n</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n<div class=\"custom-form\">\r\n<div class=\"row\">\r\n" +
                //               "<div class=\"col-md-6\">\r\n<label>Longitude (Drag marker on the map)<i class=\"fal fa-long-arrow-alt-right\"></i>  </label>\r\n<input type=\"text\" " +
                //               "placeholder=\"Map Longitude\" id=\"long\" value=\"\" />\r\n</div>\r\n<div class=\"col-md-6\">\r\n<label>Latitude (Drag marker on the map) <i class=\"fal fa-long-arrow-alt-down\"></i> " +
                //               "</label>\r\n<input type=\"text\" placeholder=\"Map Latitude\" id=\"lat\" value=\"\" />\r\n</div>\r\n</div>\r\n<div class=\"map-container\">\r\n<div id=\"singleMap\" class=\"drag-map\" data-latitude=\"40.7427837\" data-longitude=\"-73.11445617675781\"></div>\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n<div class=\"dashboard-title  dt-inbox fl-wrap\">\r\n<h3>Videos</h3>\r\n</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n<div class=\"custom-form\">\r\n<div class=\"row\">\r\n<!--col -->\r\n<div class=\"col-md-4\">\r\n<div class=\"add-list-media-wrap\">\r\n<div class=\"listsearch-input-item fl-wrap\">\r\n<div class=\"fuzone\">\r\n<form>\r\n<div class=\"fu-text\">\r\n<span><i class=\"fal fa-images\"></i> Click here or drop files to upload</span>\r\n<div class=\"photoUpload-files fl-wrap\"></div>\r\n</div>\r\n##1271##\r\n</form>\r\n</div>\r\n</div>\r\n</div>\r\n</div>\r\n<!--col end-->\r\n<!--col -->\r\n<div class=\"col-md-8\">\r\n<div class=\"add-list-media-wrap\">\r\n<div class=\"listsearch-input-item fl-wrap\">\r\n<div class=\"fuzone\">\r\n<form>\r\n<div class=\"fu-text\">\r\n<span><i class=\"fal fa-images\"></i> Click here or drop files to upload</span>\r\n<div class=\"photoUpload-files fl-wrap\"></div>\r\n</div>\r\n##1272##\r\n</form>\r\n</div>\r\n</div>\r\n</div>\r\n</div>\r\n<!--col end-->\r\n</div>\r\n</div>\r\n</div>\r\n<div class=\"dashboard-title  dt-inbox fl-wrap\">\r\n<h3>Radio</h3>\r\n</div>\r\n<div class=\"profile-edit-container fl-wrap block_box\">\r\n<div class=\"custom-form\">\r\n<div class=\"row\">\r\n<!--col -->\r\n##1273##\r\n<!--col end-->\r\n</div>\r\n</div>\r\n</div>";

                FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + "\\AdsTemplates\\48.txt");
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
                var fields = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Include(c => c.FieldType).Where(c => c.ClassifiedAdsCategoryId == 59).OrderBy(c => c.SortOrder).ToList();


                //Add classified Ad
                ClassifiedAd classifiedAd = new ClassifiedAd()
                {
                    IsActive = true,
                    ClassifiedAdsCategoryId = 59,
                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    Views = 0
                };

                _context.ClassifiedAds.Add(classifiedAd);
                _context.SaveChanges();
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

        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
