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
    public class ProductCategoryDeleteModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }
        public ProductCategoryDeleteModel(CRMDBContext Context, IToastNotification toastNotification, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }

        [BindProperty]
        public ProductCategory ProductCategory { get; set; }

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
            else
            {
                ProductCategory = productCategory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteProductCategory()
        {

            if (ProductCategory.ProductCategoryId == null || _context.ProductCategories == null)
            {
                return NotFound();
            }
            var productCategory = await _context.ProductCategories.FindAsync(ProductCategory.ProductCategoryId);
            var ClassifiedBusinessId = productCategory.ClassifiedBusinessId;

            if (productCategory != null)
            {
                ProductCategory = productCategory;
                _context.ProductCategories.Remove(ProductCategory);
                await _context.SaveChangesAsync();

                var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "/" + productCategory.Pic);

                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }

                _toastNotification.AddSuccessToastMessage("Product Category Deleted successfully");

            }
            else
            {
                _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
            }

            return RedirectToPage("./ProductCategoryIndex", new { ClassifiedBusinessId });

            
            
        }
    }
}
