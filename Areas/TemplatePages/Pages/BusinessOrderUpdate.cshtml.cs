using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System.Net.Http;
using Vision.Areas.TemplatePages.Pages;
using Vision;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.TemplatePages.Pages
{
    public class BusinessOrderUpdateModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public HttpClient httpClient { get; set; }
        public BusinessOrderUpdateModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        [BindProperty]
        public Order order {get; set;}
        public int orderId { get; set;}
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }
                order = _context.Orders.Where(a => a.OrderId == id).FirstOrDefault();


            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }

        public async Task<IActionResult> OnPostEditProductCategory()
        {
            try
            {
                var model = await _context.Orders.Where(c => c.OrderId == order.OrderId).FirstOrDefaultAsync();
                if (model == null)
                {
                    _toastNotification.AddErrorToastMessage("Order Not Found");

                    return RedirectToPage("./BusinessOrderUpdate", new { order.OrderId });
                }

                model.Adress = order.Adress;
                model.OrderNet = order.OrderNet;
                model.OrderNotes = order.OrderNotes;
                model.Discount = order.Discount;
                model.Avenue = order.Avenue;
                model.IsDeliverd = order.IsDeliverd;
                model.IsCancelled = order.IsCancelled;
                model.Deliverycost = order.Deliverycost;
                model.Governorate = order.Governorate;
                model.Area = order.Area;
                model.Piece = order.Piece;
                model.Avenue = order.Avenue;
                model.Street = order.Street;
                model.Building = order.Building;
                model.Floor = order.Floor;
                model.ApartmentNumber = order.ApartmentNumber;
                orderId = order.OrderId;
                var UpdatedOrder = _context.Orders.Attach(model);

                UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Order Edited successfully");

                return RedirectToPage("./BusinessOrderUpdate", new { orderId });

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return RedirectToPage("./BusinessOrderUpdate", new { orderId });
        }
    }
}




