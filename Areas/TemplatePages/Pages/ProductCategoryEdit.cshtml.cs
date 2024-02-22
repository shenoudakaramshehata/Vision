using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System.Net.Http;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;

namespace Vision.Areas.TemplatePages.Pages
{
    public class ProductCategoryEditModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }

        public ProductCategoryEditModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; } = default!;
        public int CategoriesCount { get; set; }
        [BindProperty]
        public bool IsActive { get; set; }
        [BindProperty]
        public long classifiedBusinessesId { get; set; }
        public async Task<IActionResult> OnGetAsync(long? ProductCategoryId)
        {
            if (ProductCategoryId == null || _context.ProductCategories == null)
            {
                return NotFound();
            }

            var productCategory = await _context.ProductCategories.FirstOrDefaultAsync(m => m.ProductCategoryId == ProductCategoryId);
            if (productCategory == null)
            {
                return NotFound();
            }
            ProductCategory = productCategory; 
            classifiedBusinessesId = productCategory.ClassifiedBusinessId;
            CategoriesCount = _context.ProductCategories.Where(a => a.ClassifiedBusinessId == classifiedBusinessesId).Count();

            return Page();
        }

        public async Task<IActionResult> OnPostEditProductCategory(IFormFile file)
        {
            var ClassifiedBusinessId = ProductCategory.ClassifiedBusinessId;
            try
            {
                var model = await _context.ProductCategories.Where(c => c.ProductCategoryId == ProductCategory.ProductCategoryId).FirstOrDefaultAsync();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Product Category Not Found");

                    return RedirectToPage("./ProductCategoryIndex", new { ClassifiedBusinessId });
                }


                if (file != null)
                {


                    string folder = "Images/ProductCategory/";

                    model.Pic = UploadImage(folder, file);
                }
                else
                {
                    model.Pic = ProductCategory.Pic;
                }

                model.Isactive = IsActive;
                model.TitleEn = ProductCategory.TitleEn;
                model.TitleAr = ProductCategory.TitleAr;
                model.SortOrder = ProductCategory.SortOrder;

                var UpdatedCategory =  _context.ProductCategories.Attach(model);

                UpdatedCategory.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                 _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Product Category Edited successfully");

                return RedirectToPage("./ProductCategoryIndex", new { ClassifiedBusinessId });

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return RedirectToPage("./ProductCategoryIndex" , new { ClassifiedBusinessId });
        }

        private bool FieldTypeExists(long ProductCategoryId)
        {
            return _context.ProductCategories.Any(e => e.ProductCategoryId == ProductCategoryId);
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
