using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using NToastNotify;
using Microsoft.EntityFrameworkCore;

namespace Vision.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification { get; }
        [BindProperty]
        public Product AddProduct { get; set; }
        public ProductPrice AddProductPrice { get; set; }
        public ProductExtra ProductExtra { get; set; }
        public static List<ProductPrice> productPrices = new List<ProductPrice>();
        public static List<ProductExtra> productExtras = new List<ProductExtra>();
        public  List<ProductType> ProductTypes = new List<ProductType>();
        public  List<ProductCategory> Categories = new List<ProductCategory>();
        public HttpClient httpClient { get; set; }
        private static long ProductId = 1;
        public AddProductModel(CRMDBContext Context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _context = Context;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _toastNotification = toastNotification;
            AddProductPrice = new ProductPrice();
        }
        //public async Task<ActionResult> OnGet(long BDId)
        public async Task<ActionResult> OnGet()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Redirect("/identity/account/login");

            //}
            //var BDCheck = _context.ClassifiedBusiness.Where(a => a.UseId == user.Id && a.ClassifiedBusinessId == BDId).FirstOrDefault();

            //if (BDCheck == null)
            //{
            //    return Redirect("/PageNF");
            //}
             //ProductTypes = _context.ProductTypes.ToList();
             //Categories = _context.ProductCategories.Where(e=>e.ClassifiedBusinessId==BDId).ToList();
            return Page();
        }
        public IActionResult OnGetInfoList(int CatId)
        {
            var Result = _context.ProductTemplateConfigs.Where(e => e.ProductTypeId == CatId).OrderBy(e => e.SortOrder).Select(c => new
            {
                ClassifiedAdsCategoryId = c.ProductTypeId,
                AdTemplateFieldCaptionAr = c.ProductTemplateFieldCaptionAr,
                AdTemplateFieldCaptionEn = c.ProductTemplateFieldCaptionEn,
                FieldTypeId = c.FieldTypeId,
                AdTemplateConfigId = c.ProductTemplateConfigId,
                IsRequired = c.IsRequired,
                ValidationMessageAr = c.ValidationMessageAr,
                ValidationMessageEn = c.ValidationMessageEn,
                AdTemplateOptions = c.ProductTemplateOptions
            }).ToList();


            return new JsonResult(Result);
        }
        public async Task<ActionResult> OnPostAddProductMedia(IFormCollection MyUploader)
        {
            try
            {
                var ProductObj = _context.Products.Include(e=>e.ProductCategory).Where(e=>e.ProductId==ProductId).FirstOrDefault();
                if (ProductObj == null)
                {
                    _toastNotification.AddErrorToastMessage("Something Went Error...Please Try Again");
                    return new JsonResult(false);
                }
                var groupedFilesList = MyUploader.Files.GroupBy(e => e.FileName).Select(grp => grp.ToList()).ToList();
                foreach (var group in groupedFilesList)
                {
                    int TemplateConfigId = 0;
                    bool CheckConvert = int.TryParse(group[0].FileName, out TemplateConfigId);
                    if (CheckConvert)
                    {
                        var contentListMediaValue = new List<ProductContentValue>();
                        foreach (var file in group)
                        {
                                string folder = "Images/Product/";
                                var contentObj = new ProductContentValue();
                                contentObj.ContentValue = UploadImage(folder, file);
                                contentListMediaValue.Add(contentObj);
                        }
                        var ContentValueObj = new ProductContent()
                        {
                            ProductId = ProductObj.ProductId,
                            ProductTemplateConfigId = TemplateConfigId,
                            ProductContentValues = contentListMediaValue

                        };
                        _context.ProductContents.Add(ContentValueObj);
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (group[0].FileName == "MainPic")
                        {

                            string folder = "Images/Product/";
                            ProductObj.MainPic = UploadImage(folder, group[0]);
                            _context.Attach(ProductObj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _context.SaveChanges();
                            
                        }
                    }
                }
               
                _toastNotification.AddSuccessToastMessage("Product Added Successfully");
                var bussinessId = ProductObj.ProductCategory.ClassifiedBusinessId;
                return new JsonResult(bussinessId);
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error...Please Try Again");
                return new JsonResult(false);
            }

            

        }
        
        public async Task<ActionResult> OnPostAddProduct([FromBody] AddProductVm addProductVm)
        {

            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _toastNotification.AddSuccessToastMessage("Please Login First");
                    return new JsonResult(false);
                }
                var productType = _context.ProductTypes.Find(addProductVm.ProductTypeId);
                if (productType == null)
                {
                    _toastNotification.AddSuccessToastMessage("Category Not Found");

                }
                
                Product product = new Product()
                {
                    IsActive = addProductVm.IsActive,
                    TitleEn = addProductVm.ProductName,
                    SortOrder = addProductVm.SortOrder,
                    ProductTypeId  = addProductVm.ProductTypeId,
                    IsFixedPrice = addProductVm.IsFixedPrice,
                    ProductCategoryId = addProductVm.ProductCategoryId,

                };
                //IFormFile ProductPic = Request.Form.Files["mainPic"];
                //if (ProductPic != null)
                //{
                //    string folder = "Images/Product/";
                //    product.MainPic = UploadFile(folder, ProductPic);
                //}
                if (addProductVm.IsFixedPrice == true)
                {
                    product.Price = addProductVm.Price;
                }
                else
                {
                    product.ProductPrices = productPrices;
                }
                product.ProductExtras = productExtras;
                _context.Products.Add(product);
                _context.SaveChanges();
                ProductId = product.ProductId;
                foreach (var item in addProductVm.addContentVMs)
                {
                    var contentList = new List<ProductContentValue>();

                    foreach (var elem in item.Values)
                    {

                        var contentObj = new ProductContentValue()
                        {
                            ContentValue = elem,
                        };
                        contentList.Add(contentObj);
                    }


                    var ContentValue = new ProductContent()
                    {
                        ProductId = product.ProductId,
                        ProductTemplateConfigId = item.AdTemplateConfigId,
                        ProductContentValues = contentList

                    };
                    _context.ProductContents.Add(ContentValue);
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
        public IActionResult OnPostFillProductPriceList([FromBody] List<ProductPrice> ProductPriceList)
        {
            if (ProductPriceList == null)
            {
                return Page();
            }
            productPrices=ProductPriceList;
            return new JsonResult(ProductPriceList);
        }
        public IActionResult OnPostFillProductExtraList([FromBody] List<ProductExtra> ProductExtraList)
        {
            if (ProductExtraList == null)
            {
                return Page();
            }
            productExtras = ProductExtraList;
            return new JsonResult(ProductExtraList);
        }
        
        private string UploadImage(string folderPath, IFormFile file)
        {
            string[] firstNames = file.ContentType.Split("/", StringSplitOptions.None);
            folderPath += Guid.NewGuid().ToString() + "_" + firstNames[0] + "." + firstNames[1];

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
        private string UploadFile(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }


    }
}

