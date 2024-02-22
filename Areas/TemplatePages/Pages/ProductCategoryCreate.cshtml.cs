using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System.Net.Http;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class ProductCategoryCreateModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }
        public int CategoriesCount { get; set; }
        [BindProperty]
        public bool IsActive { get; set; }
        [BindProperty]
        public long classifiedBusinessesId { get; set; }
        [BindProperty]
        public ProductCategory ProductCategory { get; set; }
        public ProductCategoryCreateModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }

        public async Task<IActionResult> OnGet(long BusinessId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");

                }
                CategoriesCount = _context.ProductCategories.Where(a => a.ClassifiedBusinessId == BusinessId).Count();
                classifiedBusinessesId = BusinessId;

                return Page();
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }

        public async Task<IActionResult> OnPostCreateProductCategory(IFormFile file)
        {
            ProductCategory productCategory = new ProductCategory()
            {
                Isactive = IsActive,
                TitleAr = ProductCategory.TitleAr,
                TitleEn = ProductCategory.TitleEn,
                SortOrder = ProductCategory.SortOrder,
            };
            productCategory.ClassifiedBusinessId = classifiedBusinessesId;
            if (file != null)
            {
                string folder = "Images/ProductCategory/";
                var image = UploadImage(folder, file);
                productCategory.Pic = image;
            }

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            _toastNotification.AddSuccessToastMessage("Product Category Added successfully");
            return RedirectToPage("./ProductCategoryIndex", new  { classifiedBusinessesId });
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
