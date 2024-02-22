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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Vision.Data;
using Vision.Models;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Localization;
using Vision.ViewModels;
using System.Net.Http.Headers;

namespace Vision.Areas.TemplatePages.Pages
{
    public class PostBusinessModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRazorPartialToStringRenderer _renderer;
        private List<BusinessContentValue> contentList = new List<BusinessContentValue>();
        public List<BusinessTemplateConfig> lstColumns { get; set; }
        [BindProperty]
        public ClassifiedBusiness ClassifiedBusinessobj { get; set; }
        private readonly IWebHostEnvironment _hostEnvironment;

        public HttpClient httpClient { get; set; }
        public string FormTemplate { get; set; }
        public List<BussinessPlan> bussinessPlans { get; set; }
        public PostBusinessModel(IConfiguration configuration,IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, CRMDBContext Context, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _renderer = renderer;
            _configuration = configuration;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            ClassifiedBusinessobj = new ClassifiedBusiness();

        }

		public async Task<IActionResult> OnGet(int CategoryId)
		{
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");

                }

                var BusinessCategory = _context.BusinessCategories.Where(e => e.BusinessCategoryId == CategoryId).FirstOrDefault();

                if (BusinessCategory != null)
                {
                    lstColumns = _context.BusinessTemplateConfigs.Include(c => c.BusinessTemplateOptions).Include(c => c.FieldType).Where(c => c.BusinessCategoryId == CategoryId).OrderBy(c => c.SortOrder).ToList();
                }
                bussinessPlans = _context.BussinessPlans.Where(e => e.IsActive == true).ToList();
                
                //FormTemplate = " <div class=\"container\">\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##11##\r\n</div>\r\n<div class=\"col-sm\">\r\n##15##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##27##\r\n</div>\r\n<div class=\"col-sm\">\r\n##34##\r\n</div>\r\n\r\n</div>\r\n<div class=\"row\">\r\n\r\n<div class=\"col-sm\">\r\n##35##\r\n</div>\r\n<div class=\"col-sm\">\r\n##36##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##37##\r\n</div>\r\n<div class=\"col-sm\">\r\n##41##\r\n</div>\r\n\r\n</div>\r\n<div class=\"row\">\r\n<div class=\"col-sm\">\r\n##47##\r\n</div>\r\n<div class=\"col-sm\">\r\n##50##\r\n</div>\r\n</div>\r\n<div class=\"row\">\r\n\r\n<div class=\"col-sm\">\r\n##46##\r\n</div>\r\n<div class=\"col-sm\">\r\n##39##\r\n</div>\r\n</div>\r\n</div>";
                var business = _context.ClassifiedBusiness.FirstOrDefault();
                bool isFound = System.IO.File.Exists(_hostEnvironment.WebRootPath + $"\\BusinessTemplates\\{CategoryId}.txt");
                if (isFound==false)
                {
                    return Redirect("/PageNF");
                }
                    FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + $"\\BusinessTemplates\\{CategoryId}.txt");


                var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                var BrowserCulture = locale.RequestCulture.UICulture.ToString();
                
                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "333"), await _renderer.RenderPartialToStringAsync("BusinessDisplayImage", business));

                foreach (var col in lstColumns)
                {
                    switch (col.FieldType.FieldTypeTitle)
                    {
                        case "Text":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayText", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComBusinessDisplayARText", col));

                            }
                            break;
                        case "Number":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayNumber", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARNumber", col));

                            }

                            break;
                        case "Lookup":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayLookup", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARLookup", col));

                            }

                            break;
                        case "DateTime":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayDateTime", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARDateTime", col));

                            }
                            break;
                        case "CheckBox":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayCheckBox", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARCheckBox", col));

                            }
                            break;
                        case "CheckBoxGroup":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayCheckBoxGroup", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARCheckBoxGroup", col));

                            }
                            break;
                        case "LargeText":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayLargeText", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARLargeText", col));

                            }
                            break;
                        case "Image":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayImage", col));
                            break;
                        case "Images":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayImages", col));
                            break;
                        case "Video":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayVideo", col));
                            break;

                        case "Videos":
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayVideos", col));
                            break;
                        //case "Media":
                        //    Html.Partial("~/Pages/Shared/Components/InputForm/ComDisplayMedia.cshtml", col);
                        //    break;
                        case "RadioGroup":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayRadioGroup", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARRadioGroup", col));

                            }

                            break;
                        case "Map (Location)":
                            if (BrowserCulture == "en-US")
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayMap", col));
                            }
                            else
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BusinessDisplayARMap", col));
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

        public async Task<ActionResult> OnPost(IFormFile file)
        {
            bussinessPlans = _context.BussinessPlans.Where(e => e.IsActive == true).ToList();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }

                //Loop columns and get form object by id
                var fields = _context.BusinessTemplateConfigs.Include(c => c.BusinessTemplateOptions).Include(c => c.FieldType).Where(c => c.BusinessCategoryId == 1).OrderBy(c => c.SortOrder).ToList();
                List<BusinessWorkingHours> businessWorkingHours = SetBusinessHours();

                var plan = int.Parse(Request.Form["bussinessPlans"]);
                var planBd = _context.BussinessPlans.Where( e => e.BussinessPlanId == plan).FirstOrDefault();
                //Add classified Business
                ClassifiedBusiness classifiedBusiness = new ClassifiedBusiness()
                {
                    IsActive = true,
                    BusinessCategoryId = 1,
                    Title = ClassifiedBusinessobj.Title,
                    Description = ClassifiedBusinessobj.Description,

                    PublishDate = DateTime.Now,
                    UseId = user.Id,
                    Views = 0
                };
                classifiedBusiness.businessWorkingHours = businessWorkingHours;

                if (file != null)
                {
                    string folder = "Images/BusinessMedia/";
                    classifiedBusiness.Mainpic = UploadImage(folder, file);
                }

                _context.ClassifiedBusiness.Add(classifiedBusiness);
                _context.SaveChanges();

                var businessSubscription = new BusiniessSubscription();
                if ( planBd != null)
                {
                    businessSubscription = new BusiniessSubscription()
                    {
                        IsActive = false,
                        BussinessPlanId = plan,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(planBd.DurationInMonth.Value),
                        PaymentMethodId = 1,
                        ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId,
                        Price = planBd.Price,
                    };
                    _context.BusiniessSubscriptions.Add(businessSubscription);
                    _context.SaveChanges();
                }
               
                long ClassifiedBusinessId = classifiedBusiness.ClassifiedBusinessId;
                foreach (BusinessTemplateConfig field in fields)
                {

                    int ConfigId = field.BusinessTemplateConfigId;

                    var Inputvalue = Request.Form[ConfigId.ToString()];


                    //SeparateWorkHours(Day, ClassifiedBusinessId);


                    Console.WriteLine("Field:" + field.BusinessTemplateFieldCaptionEn + "----" + "User Input is:" + Inputvalue.ToString());

                    switch (field.FieldTypeId)
                    {
                        case 1:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 2:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 3:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 4:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 5:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 6:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 7:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 8:
                            //string fileN = ConfigId.ToString();
                            //IFormFile file = Request.Form.Files[fileN];
                            //addSingleFile(file, ConfigId, ClassifiedBusinessId);
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
                            addMultiFiles(imagesFiles, ConfigId, ClassifiedBusinessId);
                            break;
                        case 10:
                            string configVedio = ConfigId.ToString();
                            IFormFile filevedio = Request.Form.Files[configVedio];
                            addSingleFile(filevedio, ConfigId, ClassifiedBusinessId);
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
                            addMultiFiles(videosFiles, ConfigId, ClassifiedBusinessId);
                            break;
                        case 13:
                            addContentValue(Inputvalue, ConfigId, ClassifiedBusinessId);
                            break;
                        case 14:

                            string Mapvalue = Request.Form["LatId"];
                            addMapValue(Mapvalue, ConfigId, ClassifiedBusinessId);
                            break;

                        default:
                            break;
                    }

                    _context.SaveChanges();
                }
                int count = contentList.Count();
                string g = "";
                if (businessSubscription.PaymentMethodId == 1)
                {
                    bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                    var TestToken = _configuration["TestToken"];
                    var LiveToken = _configuration["LiveToken"];
                    if (Fattorahstatus) // fattorah live
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = classifiedBusiness.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = businessSubscription.Price,
                            CallBackUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahBusinessPlantSuccess",
                            ErrorUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahBusinessPlanFalied",
                            UserDefinedField = businessSubscription.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://api.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Redirect(InvoiceRes.InvoiceURL);


                        }
                        else
                        {

                            _context.BusiniessSubscriptions.Remove(businessSubscription);

                            return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                        }
                    }
                    else               //fattorah test
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = classifiedBusiness.Title,
                            NotificationOption = "LNK",
                            InvoiceValue = businessSubscription.Price,
                            CallBackUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahBusinessPlantSuccess",
                            ErrorUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahBusinessPlanFalied",
                            UserDefinedField = businessSubscription.BusiniessSubscriptionId
                        };
                        var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                        string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                        var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                        var responseMessage = httpClient.PostAsync(url, httpContent);
                        var res = await responseMessage.Result.Content.ReadAsStringAsync();
                        var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                        if (FattoraRes.IsSuccess == true)
                        {
                            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                            var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                            return Redirect(InvoiceRes.InvoiceURL);
                        }
                        else
                        {

                            _context.BusiniessSubscriptions.Remove(businessSubscription);

                            return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                        }
                    }



                }


                return default;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void addContentValue(string value, int BusinessTemplateConfigId, long ClassifiedBusinessId)
        {

            List<BusinessContentValue> businessContentValues = new List<BusinessContentValue>();
            if (value != null)
            {
                var values = value.Split(",");
                for (int i = 0; i < values.Length; i++)
                {
                    var contentObj = new BusinessContentValue()
                    {
                        ContentValue = values[i],
                    };
                    businessContentValues.Add(contentObj);
                }
                BusinessContent businessContent = new BusinessContent()
                {
                    ClassifiedBusinessId = ClassifiedBusinessId,
                    BusinessTemplateConfigId = BusinessTemplateConfigId,
                    BusinessContentValues = businessContentValues
                };
                _context.BusinessContents.Add(businessContent);
                _context.SaveChanges();

            }

        }
        public void addMapValue(string value, int BusinessTemplateConfigId, long ClassifiedBusinessId)
        {

            List<BusinessContentValue> businessContentValues = new List<BusinessContentValue>();
            var contentObj = new BusinessContentValue()
            {
                ContentValue = value,
            };
            businessContentValues.Add(contentObj);
            BusinessContent businessContent = new BusinessContent()
            {
                ClassifiedBusinessId = ClassifiedBusinessId,
                BusinessTemplateConfigId = BusinessTemplateConfigId,
                BusinessContentValues = businessContentValues
            };
            _context.BusinessContents.Add(businessContent);
            _context.SaveChanges();

        }

        public void addMultiFiles(List<IFormFile> files, int BusinessTemplateConfigId, long ClassifiedAdId)
        {
            List<BusinessContentValue> BusinessContentValues = new List<BusinessContentValue>();
            foreach (var item in files)
            {
                if (item != null)
                {
                    string folder = "Images/BusinessMedia/";
                    var contentObj = new BusinessContentValue();
                    contentObj.ContentValue = UploadImage(folder, item);
                    BusinessContentValues.Add(contentObj);
                }

            }


            var BusinessContentObj = new BusinessContent()
            {
                ClassifiedBusinessId = ClassifiedAdId,
                BusinessTemplateConfigId = BusinessTemplateConfigId,
                BusinessContentValues = BusinessContentValues

            };
            _context.BusinessContents.Add(BusinessContentObj);
            _context.SaveChanges();
        }
        public void addSingleFile(IFormFile file, int BusinessTemplateConfigId, long ClassifiedAdId)
        {
            List<BusinessContentValue> BusinessContentValues = new List<BusinessContentValue>();

            if (file != null)
            {
                string folder = "Images/BusinessMedia/";
                var contentObj = new BusinessContentValue();
                contentObj.ContentValue = UploadImage(folder, file);
                BusinessContentValues.Add(contentObj);
            }


            var ContentValueObj = new BusinessContent()
            {
                ClassifiedBusinessId = ClassifiedAdId,
                BusinessTemplateConfigId = BusinessTemplateConfigId,
                BusinessContentValues = BusinessContentValues

            };
            _context.BusinessContents.Add(ContentValueObj);
            _context.SaveChanges();
        }

        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }

        private void SeparateWorkHours(string value, long BusinessId)
        {
            List<BusinessWorkingHours> BusinessWorkingHours = new List<BusinessWorkingHours>();
            if (value != null)
            {
                var values = value.Split(",");
                foreach (var val in values)
                {
                    var model = JsonConvert.DeserializeObject<BusinessWorkingHours>(val);
                    //var BusinessWorkingHoursObj = new BusinessWorkingHours()
                    //{
                    //    Day = model.Day,
                    //    StartTime1 = model.StartTime1,
                    //    EndTime1 = model.EndTime1,
                    //    StartTime2 = model.StartTime2,
                    //    EndTime2 = model.EndTime2,
                    //    Isclosed = model.Isclosed,
                    //};
                    BusinessWorkingHours.Add(model);
                }




            }
        }

        private List<BusinessWorkingHours> SetBusinessHours()
        {
            List<BusinessWorkingHours> businessWorkingHours = new List<BusinessWorkingHours>();
            var daySat = Request.Form["SatClosed"];
            var daySun = Request.Form["SunClosed"];

            if (Request.Form["SatClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["SatDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["SatFromTime1"],
                    EndTime1 = Request.Form["SatEndTime1"],
                    StartTime2 = Request.Form["SatFromTime2"],
                    EndTime2 = Request.Form["SatEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["SatDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["SunClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["SunDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["SunFromTime1"],
                    EndTime1 = Request.Form["SunEndTime1"],
                    StartTime2 = Request.Form["SunFromTime2"],
                    EndTime2 = Request.Form["SunEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["SunDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["MonClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["MonDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["MonFromTime1"],
                    EndTime1 = Request.Form["MonEndTime1"],
                    StartTime2 = Request.Form["MonFromTime2"],
                    EndTime2 = Request.Form["MonEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["MonDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["TueClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["TueDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["TueFromTime1"],
                    EndTime1 = Request.Form["TueEndTime1"],
                    StartTime2 = Request.Form["TueFromTime2"],
                    EndTime2 = Request.Form["TueEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["TueDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["WedClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["WedDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["WedFromTime1"],
                    EndTime1 = Request.Form["WedEndTime1"],
                    StartTime2 = Request.Form["WedFromTime2"],
                    EndTime2 = Request.Form["WedEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["WedDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["ThuClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["ThuDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["ThuFromTime1"],
                    EndTime1 = Request.Form["ThuEndTime1"],
                    StartTime2 = Request.Form["ThuFromTime2"],
                    EndTime2 = Request.Form["ThuEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["ThuDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }

            if (Request.Form["FriClosed"] == "on")
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = null,
                    EndTime1 = null,
                    StartTime2 = null,
                    EndTime2 = null,
                    Isclosed = true,
                    Day = Request.Form["FriDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            else
            {
                var businessWorkingHour = new BusinessWorkingHours()
                {
                    StartTime1 = Request.Form["FriFromTime1"],
                    EndTime1 = Request.Form["FriEndTime1"],
                    StartTime2 = Request.Form["FriFromTime2"],
                    EndTime2 = Request.Form["FriEndTime2"],
                    Isclosed = false,
                    Day = Request.Form["FriDay"]
                };
                businessWorkingHours.Add(businessWorkingHour);
            }
            return businessWorkingHours;
        }
    }
}
