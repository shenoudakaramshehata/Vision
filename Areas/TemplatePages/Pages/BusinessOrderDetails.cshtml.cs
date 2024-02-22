using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;
using static NuGet.Packaging.PackagingConstants;

namespace Vision.Areas.TemplatePages.Pages
{
    public class BusinessOrderDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public HttpClient httpClient { get; set; }
        public BusinessOrderDetailsModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public Order order {get; set;}
        public double cartSubTotal { get; set; }
        [BindProperty]
        public List<ShoppingCart> cartProducts { get; set; }
        [BindProperty]
        public int productQuantity { get; set; }
        public ShoppingCart? cartShippingCost { get; set; }
        public List<ShopingCartProductExtraFeatures> productExtraFeatures { get; set; }
        public double totalCost { get; set; }

        public ApplicationUser user { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }
                order = _context.Orders.Where(a => a.OrderId == 8).FirstOrDefault();


            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }
    }
}
