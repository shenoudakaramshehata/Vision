using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NToastNotify;
using Vision.Areas.CRM.Pages.Configurations.ManageClasifiedChart;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageBusinessChart
{
    public class IndexModel : PageModel
    {
        public string TreeViewJSON { get; set; }

        private CRMDBContext _context { get; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        [BindProperty]
        public BusinessCategory AddbusinessCategory { get; set; }
        [BindProperty]
        public BusinessTemplateConfig BusinessTemplateConfig { get; set; }

        public List<BusinessCategory> BusinessCategories = new List<BusinessCategory>();
        public List<FieldType> fieldTypes = new List<FieldType>();

        public List<BusinessTemplateConfig> BusinessTemplateConfigList = new List<BusinessTemplateConfig>();
        public static List<BusinessTemplateConfig> StaticBusinessTemplateConfigsList = new List<BusinessTemplateConfig>();
        public string Url { get; set; }
        static int ClassifiedBusinessCatId = 0;
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

            var result = _context.BusinessCategories.ToList();
            BusinessCategories = result;
            fieldTypes = _context.FieldTypes.ToList();
            
            foreach (BusinessCategory type in result)
            {
                if (BrowserCulture == "en-US")
                {
                    nodes.Add(new TreeViewNode { id = type.BusinessCategoryId.ToString(), parent = "#", text = type.BusinessCategoryTitleEn });

                }
                else
                {
                    nodes.Add(new TreeViewNode { id = type.BusinessCategoryId.ToString(), parent = "#", text = type.BusinessCategoryTitleAr });


                }
            }


            this.TreeViewJSON = JsonConvert.SerializeObject(nodes);
            BusinessTemplateConfigList = StaticBusinessTemplateConfigsList;

        }

      

        public IActionResult OnPost(IFormFile file)
        {
            try
            {
                if (AddbusinessCategory.BusinessCategoryParentId == 0)
                {
                    AddbusinessCategory.BusinessCategoryParentId = null;
                }
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
                    return Redirect("/CRM/Configurations/ManageBusinessChart/Index");
                }
                if (file != null)
                {
                    string folder = "Images/BusinessCategory/";
                    AddbusinessCategory.BusinessCategoryCategoryPic = UploadImage(folder, file);
                }
                _context.BusinessCategories.Add(AddbusinessCategory);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Business Category Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageBusinessChart/Index");
        }

        public IActionResult OnGetFillTemplateList(int id)
        {
            ClassifiedBusinessCatId = id;
            BusinessTemplateConfigList = _context.BusinessTemplateConfigs.Include(e => e.FieldType).Include(e => e.BusinessCategories).Where(c => c.BusinessCategoryId == id).ToList();
            StaticBusinessTemplateConfigsList = BusinessTemplateConfigList;

            return new JsonResult(BusinessTemplateConfigList);
        }

        public async Task<IActionResult> OnPostAddAttribute()
        {
           
            //if (!ModelState.IsValid)
            //{
            //    _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
            //    return Redirect("/CRM/Configurations/ManageBusinessChart/Index");
            //}
            try
            {
                var collectionsSelection = Request.Form["Multipleselection"];

                foreach (var item in collectionsSelection)
                {
                    var collectionId = int.Parse(item);
                    var Config = new BusinessTemplateConfig()
                    {
                        BusinessCategoryId= collectionId,
                        BusinessTemplateFieldCaptionAr= BusinessTemplateConfig.BusinessTemplateFieldCaptionAr,
                        BusinessTemplateFieldCaptionEn= BusinessTemplateConfig.BusinessTemplateFieldCaptionEn,
                        FieldTypeId= BusinessTemplateConfig.FieldTypeId,
                        IsRequired= BusinessTemplateConfig.IsRequired,
                        SortOrder= BusinessTemplateConfig.SortOrder,
                        ValidationMessageAr= BusinessTemplateConfig.ValidationMessageAr,
                        ValidationMessageEn= BusinessTemplateConfig.ValidationMessageEn
                    };

                    _context.BusinessTemplateConfigs.Add(Config);
                    _context.SaveChanges();

                }

                _toastNotification.AddSuccessToastMessage("Attribute Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageBusinessChart/Index");
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

