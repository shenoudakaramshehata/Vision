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
    public class ListingReviewsModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public UserManager<ApplicationUser> UserManager { get; }
        public List<Review> reviewlist = new List<Review>();
        public static List<Review> staticreviewlist = new List<Review>();
        public static List<Review> Allreviwes = new List<Review>();
        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public ListingReviewsModel(UserManager<ApplicationUser> userManager, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            UserManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> OnPostBranchesList([FromBody] int num)
        {
            var user = await UserManager.GetUserAsync(User);
            var listingid = _context.AddListings.Where(a => a.CreatedByUser == user.Email);
            foreach (var item in listingid)
            {
                //var rev = _context.Reviews.Where(a => a.AddListingId == item.AddListingId);
                var rev = _context.Reviews.ToList();
                reviewlist.AddRange(rev);
            }
            var start = (num - 1) * 8;
            staticreviewlist = reviewlist.Skip(start).Take(5).ToList();
            reviewlist = staticreviewlist;
            return new JsonResult(reviewlist);
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/login");
            }
            try
            {
                var listingid = _context.AddListings.Where(a => a.CreatedByUser == user.Email);
                if (first)
                {
                    foreach (var item in listingid)
                    {
                        //var rev = _context.Reviews.Where(a => a.AddListingId == item.AddListingId);
                        var rev = _context.Reviews.ToList();
                        reviewlist.AddRange(rev);
                        Allreviwes.AddRange(rev);

                    }
                    first = false;
                    staticreviewlist = reviewlist;
                    reviewlist = reviewlist.Take(8).ToList();


                }
                else
                    reviewlist = staticreviewlist;

                float number = (float)Allreviwes.Count() / 8;
                var pagenumber = Math.Ceiling(number);
                for (int i = 1; i <= pagenumber; i++)
                {
                    Pagenumbers.Add(i);
                }
                return Page();

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("somthing went Error");
            }
            return Page();


        }
    }
}
