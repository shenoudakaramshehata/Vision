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

namespace Vision.Pages
{
    public class BDOffersModel : PageModel
    {
        private CRMDBContext _context;
        public List<BDOffer> BDOffers = new List<BDOffer>();

        private UserManager<ApplicationUser> _userManager { get; }
        public static List<BDOffer> Listings2 = new List<BDOffer>();

        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public static long BussinessId = 0;
        private IToastNotification _toastNotification { get; }
        public BDOffersModel(UserManager<ApplicationUser> userManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _context = Context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> OnPostBranchesList([FromBody] int num)
        {
            var user = await _userManager.GetUserAsync(User);
            var alllistings = _context.BDOffers.Where(a => a.ClassifiedBusinessId == BussinessId).ToList();

            var start = (num - 1) * 2;
            var end = (num) * 2;
            Listings2 = alllistings.Skip(start).Take(2).ToList();
            BDOffers = Listings2;
            return new JsonResult(BDOffers);
        }
        
        public async Task<IActionResult> OnGet(long BDId)
        {
            BussinessId = BDId;
            if (first)
            {
                BDOffers = _context.BDOffers.Where(a => a.ClassifiedBusinessId == BDId).ToList();
                Listings2 = BDOffers;
                first = false;
            }
            else
                BDOffers = Listings2;
            var alllistings = _context.BDOffers.Where(a => a.ClassifiedBusinessId == BDId).ToList();

            float number = (float)alllistings.Count() / 2;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Page();
        }
        
    }
}

