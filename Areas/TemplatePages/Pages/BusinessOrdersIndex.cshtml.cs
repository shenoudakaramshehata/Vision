using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class BusinessOrdersIndexModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public HttpClient httpClient { get; set; }
        public BusinessOrdersIndexModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public List<Order> orders {get; set;}
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }
                orders = _context.Orders.Where(a => a.ClassifiedBusinessId == id && a.IsDeliverd == true).ToList();
                
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }
    }
}
