using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Vision.Data;
using Vision.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Microsoft.AspNetCore.Localization;

namespace Vision.Areas.CRM.Pages.Configurations.ManageClasifiedChart
{

#nullable disable
    public class IndexModel : PageModel
    {
        public string TreeViewJSON { get; set; }

        private CRMDBContext _context { get; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        [BindProperty]
        public ClassifiedAdsCategory AddClassifiedAdsCategory { get; set; }
        [BindProperty]
        public AdTemplateConfig adTemplateConfig { get; set; }
        public List<ClassifiedAdsCategory> classifiedAdsCategories = new List<ClassifiedAdsCategory>();
        public List<FieldType> fieldTypes = new List<FieldType>();

        public List<AdTemplateConfig> AdTemplateConfigsList = new List<AdTemplateConfig>();
        public static List<AdTemplateConfig> StaticAdTemplateConfigsList = new List<AdTemplateConfig>();
        public string Url { get; set; }
        static int ClassifiedAdsCatId = 0;
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            this._context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
        }


        public void OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            Url = $"{this.Request.Scheme}://{this.Request.Host}";
            List<TreeViewNode> nodes = new List<TreeViewNode>();

            var result = _context.ClassifiedAdsCategories.ToList();
            classifiedAdsCategories = result;
            fieldTypes = _context.FieldTypes.ToList();
            //var temp  = (IEnumerable<TreeViewNode>)result.Select(c => new TreeViewNode
            //{
            //    id =c.ClassifiedAdsCategoryId.ToString(),
            //    parent = c.ClassifiedAdsCategoryParentId.ToString(),
            //    text = c.ClassifiedAdsCategoryTitleEn.ToString(),

            //});

            

            foreach (ClassifiedAdsCategory type in result)
            {
                if (BrowserCulture == "en-US")
                {
                    nodes.Add(new TreeViewNode { id = type.ClassifiedAdsCategoryId.ToString(), parent = "#", text = type.ClassifiedAdsCategoryTitleEn });

                }
                else
                {
                    nodes.Add(new TreeViewNode { id = type.ClassifiedAdsCategoryId.ToString(), parent = "#", text = type.ClassifiedAdsCategoryTitleAr });


                }
            }


            this.TreeViewJSON = JsonConvert.SerializeObject(nodes);
            AdTemplateConfigsList = StaticAdTemplateConfigsList;

        }
        public async Task<IActionResult> OnPost(IFormFile file)
        {
            try
            {

                if (AddClassifiedAdsCategory.ClassifiedAdsCategoryParentId == 0)
                {
                    AddClassifiedAdsCategory.ClassifiedAdsCategoryParentId = null;
                }
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
                    return Redirect("/CRM/Configurations/ManageClasifiedChart/Index");
                }
                
                var categories = Request.Form["choices-multiple-defaultcat"];
                foreach (var item in categories)
                {
                    var AddClassifiedAdsCategoryobj = new ClassifiedAdsCategory();
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryDescAr = AddClassifiedAdsCategory.ClassifiedAdsCategoryDescAr;
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryDescEn = AddClassifiedAdsCategory.ClassifiedAdsCategoryDescEn;
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryIsActive = AddClassifiedAdsCategory.ClassifiedAdsCategoryIsActive;
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryParentId = int.Parse(item);
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategorySortOrder = AddClassifiedAdsCategory.ClassifiedAdsCategorySortOrder;

                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryTitleAr = AddClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr;
                    AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryTitleEn = AddClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn;

                    if (file != null)
                    {
                        string folder = "Images/Category/";
                        AddClassifiedAdsCategoryobj.ClassifiedAdsCategoryPic = UploadImage(folder, file);
                    }

                    _context.ClassifiedAdsCategories.Add(AddClassifiedAdsCategoryobj);

                }
                
                
                //_context.ClassifiedAdsCategories.Add(AddClassifiedAdsCategory);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Classified Ads Category Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageClasifiedChart/Index");
        }
        public IActionResult OnGetFillTemplateList(int id)
        {
            ClassifiedAdsCatId = id;
            AdTemplateConfigsList = _context.AdTemplateConfigs.Include(e => e.FieldType).Include(e => e.ClassifiedAdsCategory).Where(c => c.ClassifiedAdsCategoryId == id).ToList();
            StaticAdTemplateConfigsList = AdTemplateConfigsList;

            return new JsonResult(AdTemplateConfigsList);
        }
        public async Task<IActionResult> OnPostAddAttribute()
        {
            //adTemplateConfig.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr = "";
           // adTemplateConfig.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn = "";
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
                return Redirect("/CRM/Configurations/ManageClasifiedChart/Index");
            }
            try
            {
                var categories = Request.Form["choices-multiple-defaults"];

                foreach (var item in categories)
                {
                    var adTemplateConfigObj = new AdTemplateConfig();
                    adTemplateConfigObj.AdTemplateFieldCaptionAr = adTemplateConfig.AdTemplateFieldCaptionAr;
                    adTemplateConfigObj.AdTemplateFieldCaptionEn = adTemplateConfig.AdTemplateFieldCaptionEn;
                    adTemplateConfigObj.FieldTypeId = adTemplateConfig.FieldTypeId;
                    adTemplateConfigObj.ClassifiedAdsCategoryId = int.Parse(item);
                    adTemplateConfigObj.ValidationMessageAr= adTemplateConfig.ValidationMessageAr;
                    adTemplateConfigObj.ValidationMessageEn= adTemplateConfig.ValidationMessageEn;
                    adTemplateConfigObj.SortOrder = adTemplateConfig.SortOrder;
                    adTemplateConfigObj.IsRequired= adTemplateConfig.IsRequired;

                    _context.AdTemplateConfigs.Add(adTemplateConfigObj);

                }

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Attribute Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageClasifiedChart/Index");
        }

        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }

    }
    public class TreeViewNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
    }

}
