using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class BusinessOrdersCurrentModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public HttpClient httpClient { get; set; }
        public BusinessOrdersCurrentModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public bool isDelivered { get; set; }
        public bool isCancelled { get; set; }
        public int BusinessId { get; set; }
        [BindProperty]
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
                orders = _context.Orders.Where(a => a.ClassifiedBusinessId == id 
                                                    && a.IsDeliverd == false 
                                                    && a.IsCancelled == false )
                                                    .ToList();
                BusinessId = id;
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateOrderCancel()
        {
            try
            {

                var model =  _context.Orders.Where(c => c.ClassifiedBusinessId == BusinessId && c.OrderId == 99).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order not found");
                    return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });
                }

                model.IsCancelled = true;
                var UpdatedOrder = _context.Orders.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Order Updated successfully");

                return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });
        }
        public async Task<IActionResult> OnPostUpdateOrderDeliver()
        {
            try
            {

                var model = _context.Orders.Where(c => c.ClassifiedBusinessId == BusinessId && c.OrderId == 99).FirstOrDefault();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order not found");
                    return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });
                }

                model.IsDeliverd = true;
                var UpdatedOrder = _context.Orders.Attach(model);
                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Order Updated successfully");

                return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return RedirectToPage("./BusinessOrdersCurrent", new { BusinessId });
        }
    }
}
