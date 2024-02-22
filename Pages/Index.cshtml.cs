using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Email;

namespace Vision.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CRMDBContext _context;
        public List<Category> Categories { get; set; }
        public List<ClassifiedAdsCategory> TopAdsCategories { get; set; }
        public List<ClassifiedAd> LatestAds { get; set; }
        public List<AddListing> addListings { get; set; }
		private readonly IEmailSender _emailSender;
		private UserManager<ApplicationUser> UserManager { get; }

        public IndexModel(CRMDBContext context, IEmailSender emailSender, ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            UserManager = userManager;
			_emailSender = emailSender;
		}

        public async void OnGet()
        {
            TopAdsCategories = _context.ClassifiedAdsCategories
                            .Where(a => a.ClassifiedAdsCategoryParentId == null)
                            .ToList();
            
            LatestAds = _context.ClassifiedAds
                        .Where(a => a.IsActive == true)
                        .OrderByDescending(a => a.PublishDate)
                        .Take(10)
                        .ToList();


            Categories = _context.Categories.ToList();
            addListings = _context.AddListings.Include(e => e.Category).OrderByDescending(e => e.AddListingId).Take(5).ToList();
            //var message = new Message(new string[] { "shenouda0128992@gmail.com" }, "Test mail with Attachments", "This is the content from our mail with attachments.", null);
            //await _emailSender.SendEmailAsync(message);
        }
        public async Task<IActionResult> OnPostAddTFavourite([FromBody] int listingid)
        {
            bool favouriteflag = false;
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(favouriteflag);
            }
            var favouriteLi = _context.Favourites.Where(a => a.UserId == user.Id && a.AddListingId == listingid).FirstOrDefault(); ;
            if (favouriteLi == null)
            {
                var favouriteobj = new Favourite() { AddListingId = listingid, UserId = user.Id };

                _context.Favourites.Add(favouriteobj);
                favouriteflag = true;
            }
            else
                _context.Favourites.Remove(favouriteLi);
            _context.SaveChanges();
            return new JsonResult(favouriteflag);
        }

        public async Task<IActionResult> OnPostFavourite([FromBody] int Classifiedid)
        {
            bool favouriteflag = false;
            var user = await UserManager.GetUserAsync(User);
            var favourite = _context.FavouriteClassifieds.Where(a => a.UserId == user.Id && a.ClassifiedAdId == Classifiedid).FirstOrDefault();
            if (favourite == null)
            {
                var favouriteobj = new FavouriteClassified() { ClassifiedAdId = Classifiedid, UserId = user.Id };

                _context.FavouriteClassifieds.Add(favouriteobj);
                favouriteflag = true;
            }
            else
                _context.FavouriteClassifieds.Remove(favourite);
            _context.SaveChanges();
            return new JsonResult(favouriteflag);
        }

    }

}


