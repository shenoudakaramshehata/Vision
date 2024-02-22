using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class ProductCategoryDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;

        public ProductCategoryDetailsModel(CRMDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        public ProductCategory ProductCategory { get; set; }
        public async Task<IActionResult> OnGetAsync(long? ProductCategoryId)
        {
            if (ProductCategoryId == null || _context.ProductCategories == null)
            {
                return NotFound();
            }
            var productCategory = await _context.ProductCategories.Include(e => e.ClassifiedBusiness).FirstOrDefaultAsync(m => m.ProductCategoryId == ProductCategoryId);

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
    }
}
