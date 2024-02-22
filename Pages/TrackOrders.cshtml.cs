using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Pages
{
    public class TrackOrdersModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }

        public TrackOrdersModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public Order Order { get; set; }
        public ApplicationUser user { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public ShoppingCart businessCheckout { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                //user = await _userManager.GetUserAsync(User);
                //if (user == null)
                //{
                //    return Redirect("/identity/account/login");

                //}

                Order = _context.Orders.Where(x => x.OrderId == id).FirstOrDefault();
                if (Order != null)
                {
                   OrderItems = _context.OrderItems.Where(x => x.OrderId == Order.OrderId).Include(a => a.Product).ToList();
                   
                }
                else 
                {
                    _toastNotification.AddErrorToastMessage("Order not Found");
                }

                businessCheckout = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                   .Include(e => e.Product)
                   .ThenInclude(e => e.ProductCategory)
                   .ThenInclude(e => e.ClassifiedBusiness)
                   .FirstOrDefault();

                return Page();
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }
    }
}
