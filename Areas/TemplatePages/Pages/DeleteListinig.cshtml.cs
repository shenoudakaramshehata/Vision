using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Areas.TemplatePages.Pages
{
    public class DeleteListinigModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public List<Branch> BranchesList = new List<Branch>();

        [BindProperty]
        public AddListing AddListing { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }

        public DeleteListinigModel(UserManager<ApplicationUser> userManager, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            UserManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                AddListing = await _context.AddListings.Include(a => a.Branches).FirstOrDefaultAsync(m => m.AddListingId == id);
                if (AddListing == null)
                {
                    return RedirectToPage("PageNF");
                }
                BranchesList = AddListing.Branches.ToList();

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }



            return Page();
        }

        public IActionResult OnPost()
        {
            var model = _context.AddListings.Where(c => c.AddListingId == AddListing.AddListingId).FirstOrDefault();
            if (model == null)
            {
                return Page();
            }
            _context.AddListings.Remove(model);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Listing Deleted successfully");



            return Redirect("./DashBoardListingTable");

        }
    }
}

