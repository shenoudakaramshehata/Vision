using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Vision.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Areas.TemplatePages.Pages
{
    public class DisplayListingModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public static bool ajax = true;
        public DisplayListingModel(CRMDBContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            UserManager = userManager;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }

        public List<Vision.Models.AddListing> AddListingList { get; set; }
        public static List<Vision.Models.AddListing> AddListingListStatic { get; set; }
        [BindProperty]
        public ClassifiedAdsFilterModel FilterModel { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }

        public List<string> Cities = new List<string>();
        public static List<AddListing> AddListingsloc = new List<AddListing>();
        public static List<AddListing> Listings2 = new List<AddListing>();

        public int pages = 6;
        public async Task<IActionResult> OnPostPagesList([FromBody] int num)
        {

            //var alllistings = _context.AddListings.Include(a => a.Category).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToList();
            ajax = false;
            var start = (num - 1) * pages;
            AddListingList = AddListingsloc.Skip(start).Take(pages).ToList();
            Listings2 = AddListingList;
            return new JsonResult(num);
        }
         
        public async Task<IActionResult> OnPostFavourite([FromBody] int listingid)
        {
            bool favouriteflag = false ;
           var user = await UserManager.GetUserAsync(User);
            var favourite = _context.Favourites.Where(a => a.UserId == user.Id && a.AddListingId == listingid).FirstOrDefault(); ;
            if (favourite == null)
            {
                var favouriteobj = new Favourite() { AddListingId = listingid, UserId = user.Id };

                _context.Favourites.Add(favouriteobj);
                favouriteflag = true;
            }
            else
                _context.Favourites.Remove(favourite);
            _context.SaveChanges();
            return new JsonResult(favouriteflag);
        }


        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var alllist = await _context.AddListings.Include(a => a.Category).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                Pagenumbers = getpagescount(alllist);
                if (first)
                {

                    AddListingsloc = alllist;
                    AddListingList = alllist.Take(pages).ToList();
                    //var ListCities = _context.AddListings.Select(a => a.City).Distinct();
                    first = false;
                }
                else
                    AddListingList = Listings2;

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            if (AddListingListStatic != null)
            {
                if (AddListingListStatic.Count != 0)
                {
                    AddListingList = AddListingListStatic;
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostBranchesList(int x)
        {
            return new JsonResult(AddListingsloc);
        }
        public async Task<IActionResult> OnPostSortList([FromBody] List<string> SelectedValue)
        {
            AddListingListStatic = new List<Models.AddListing>();
            int restult = 0;
            bool checkValue = int.TryParse(SelectedValue[0], out restult);
            if (checkValue)
            {
                if (restult == 1)
                {
                    AddListingListStatic = await _context.AddListings.Include(a => a.Category).Include(a => a.ListingPhotos).Include(a => a.Reviews).OrderBy(e => e.Rating).ToListAsync();
                    AddListingsloc = AddListingListStatic;
                }
                if (restult == 2)
                {

                    AddListingListStatic = await _context.AddListings.Include(a => a.Category).Include(a => a.ListingPhotos).Include(a => a.Reviews).OrderByDescending(e => e.Rating).ToListAsync();
                    AddListingsloc = AddListingListStatic;
                }
            }
            var alllistings = _context.AddListings.ToList();
            float number = (float)alllistings.Count() / pages;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return new JsonResult(SelectedValue);
        }
        public List<int> getpagescount(List<AddListing> addListings)
        {

            float number = (float)addListings.Count() / pages;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Pagenumbers;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ajax==false)
            {
                AddListingList = Listings2;
                Pagenumbers = getpagescount(AddListingsloc);
                ajax = true;
                return Page();    
            }
            try
            {
                AddListingList = await _context.AddListings.Include(a => a.Category).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                if (FilterModel.CatId != 0)
                {
                    AddListingList = await _context.AddListings.Include(a => a.Category).Where(e => e.CategoryId == FilterModel.CatId).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                    AddListingsloc = AddListingList;
                }
                if (FilterModel.Target != null)
                {
                    AddListingList = await _context.AddListings.Include(a => a.Category).Where(e => e.Title.ToUpper().Contains(FilterModel.Target.ToUpper())).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                    AddListingsloc = AddListingList;

                }
                if (FilterModel.Location != null)
                {
                    AddListingList = await _context.AddListings.Include(a => a.Category).Where(e => e.MainLocataion.ToUpper().Contains(FilterModel.Location.ToUpper())).Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                    AddListingsloc = AddListingList;

                }
                if (FilterModel.City != null)
                {
                    AddListingList = await _context.AddListings.Include(a => a.Category)/*Where(e => e.City.ToUpper().Contains(FilterModel.City.ToUpper()))*/.Include(a => a.ListingPhotos).Include(a => a.Reviews).ToListAsync();
                    AddListingsloc = AddListingList;

                }
                if (FilterModel.Location == null && FilterModel.Target == null && FilterModel.CatId == 0 && FilterModel.City == null)
                {
                    AddListingList = new List<Models.AddListing>();
                }
                Pagenumbers = getpagescount(AddListingList);
                AddListingList = AddListingList.Take(pages).ToList();

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Page();
        }

    }
}

