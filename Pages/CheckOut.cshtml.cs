using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Pages
{
    public class CheckOutModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }
        public List<ShoppingCart> cartProducts { get; set; }
        public List<ShopingCartProductExtraFeatures> productExtraFeatures { get; set; }
        public ProductPrice ProductPrice { get; set; }

        public CheckOutModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public async Task<IActionResult> OnGet()
        {
            //try
            //{
            //    var user = await _userManager.GetUserAsync(User);
            //    if (user == null)
            //    {
            //        return Redirect("/identity/account/login");

            //    }

            //    cartProducts = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
            //                                         .Include(e => e.Product)
            //                                         .Include(e => e.ProductPrice)
            //                                         .Include(e => e.ShopingCartProductExtraFeatures)
            //                                         .ThenInclude(e => e.ProductExtra)
            //                                         .ToList();
                
            //}
            //catch(Exception ex)
            //{

            //}
            return Page();
        }
    }
}
