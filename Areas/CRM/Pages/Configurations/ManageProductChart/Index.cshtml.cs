using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Vision.Data;
using Vision.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Microsoft.AspNetCore.Localization;

namespace Vision.Areas.CRM.Pages.Configurations.ManageProductChart
{

#nullable disable
    public class IndexModel : PageModel
    {
        public string TreeViewJSON { get; set; }

        private CRMDBContext _context { get; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        [BindProperty]
        public ProductType AddProductType { get; set; }
        [BindProperty]
        public ProductTemplateConfig adTemplateConfig { get; set; }
        public List<ProductType> ProductTypes = new List<ProductType>();
        public List<FieldType> fieldTypes = new List<FieldType>();

        public List<ProductTemplateConfig> AdTemplateConfigsList = new List<ProductTemplateConfig>();
        public static List<ProductTemplateConfig> StaticAdTemplateConfigsList = new List<ProductTemplateConfig>();
        public string Url { get; set; }
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        public static int ProductTypeId;
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

            var result = _context.ProductTypes.ToList();
            ProductTypes = result;
            fieldTypes = _context.FieldTypes.ToList();

            foreach (ProductType type in result)
            {
                if (BrowserCulture == "en-US")
                {
                    nodes.Add(new TreeViewNode { id = type.ProductTypeId.ToString(), parent = "#", text = type.TitleEn });

                }
                else
                {
                    nodes.Add(new TreeViewNode { id = type.ProductTypeId.ToString(), parent = "#", text = type.TitleAr });


                }
            }


            this.TreeViewJSON = JsonConvert.SerializeObject(nodes);
            AdTemplateConfigsList = StaticAdTemplateConfigsList;

        }
        public async Task<IActionResult> OnPost(IFormFile file)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
                    return Redirect("/CRM/Configurations/ManageProductChart/Index");
                }
                if (file != null)
                {
                    string folder = "Images/ProductType/";
                    AddProductType.Pic = UploadImage(folder, file);
                }
                _context.ProductTypes.Add(AddProductType);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Product Type Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageProductChart/Index");
        }
        public IActionResult OnGetFillTemplateList(int id)
        {
            ProductTypeId = id;
            AdTemplateConfigsList = _context.ProductTemplateConfigs.Include(e => e.FieldType).Include(e => e.ProductType).Where(c => c.ProductTypeId == id).ToList();
            StaticAdTemplateConfigsList = AdTemplateConfigsList;

            return new JsonResult(AdTemplateConfigsList);
        }
        public async Task<IActionResult> OnPostAddAttribute()
        {
          
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Enter Required Data...Please!");
                return Redirect("/CRM/Configurations/ManageProductChart/Index");
            }
            try
            {


                _context.ProductTemplateConfigs.Add(adTemplateConfig);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Attribute Added Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error Please Try Again ....");
            }
            return Redirect("/CRM/Configurations/ManageProductChart/Index");
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
