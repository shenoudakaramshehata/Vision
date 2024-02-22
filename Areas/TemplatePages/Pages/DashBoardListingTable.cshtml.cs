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
using NToastNotify;

namespace Vision.Areas.TemplatePages.Pages
{
    
    public class DashBoardListingTableModel : PageModel
    {
        private CRMDBContext _context;
        public  List<AddListing> Listings =new List<AddListing>();
        public static List<AddListing> Listings2 =new List<AddListing>();
        public List<int> Pagenumbers =new List<int>();
        public static bool first = true;
        public UserManager<ApplicationUser> UserManager { get; }

        public DashBoardListingTableModel(UserManager<ApplicationUser> userManager, CRMDBContext Context)
        {
            UserManager = userManager;
            _context = Context;
        }
        public async Task<IActionResult> OnPostBranchesList([FromBody]int num)
        {
            var user = await UserManager.GetUserAsync(User);
            var alllistings = _context.AddListings.Where(a => a.CreatedByUser == user.Email).ToList();

            var start = (num - 1) * 5;
            var end = (num) * 5;
            Listings2 = alllistings.Skip(start).Take(5).ToList();
            Listings = Listings2;
            return new JsonResult(Listings);
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user==null)
            {
                return Redirect("/identity/account/login");

            }
            if (first)
            {
                Listings= _context.AddListings.Where(a => a.CreatedByUser == user.Email).Take(5).ToList();
                Listings2 = Listings;
                first = false;
            }else
            Listings = Listings2;

            //Listings = _context.AddListings.Where(a=>a.CreatedByUser==user.Email).ToList();
            var alllistings = _context.AddListings.Where(a => a.CreatedByUser == user.Email).ToList();

            float number = (float)alllistings.Count() / 5;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Page();
        }
    }
}
