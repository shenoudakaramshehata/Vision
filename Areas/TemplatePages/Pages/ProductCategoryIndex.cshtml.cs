using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class ProductCategoryIndexModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public HttpClient httpClient { get; set; }
        public long classifiedBusinessId { get; set; }
        public ProductCategoryIndexModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public List<ProductCategory> ProductCategory { get; set; }
        public async Task<IActionResult> OnGetAsync(long ClassifiedBusinessId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");

                }
                if (_context.ProductCategories != null)
                {
                    ProductCategory = await _context.ProductCategories.Where(e => e.ClassifiedBusinessId == ClassifiedBusinessId).ToListAsync();
                    classifiedBusinessId = ClassifiedBusinessId;
                }
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();

        }
    }
}
