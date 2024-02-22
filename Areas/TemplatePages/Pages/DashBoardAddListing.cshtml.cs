using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision.Areas.TemplatePages.Pages
{
    public class DashBoardAddListingModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public static List<Branch> Branches;
        public List<City>Cities=new List<City>(); 
        public static List<City>staticCitiesList;


        [BindProperty]
        public AddListing AddListing { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
  
        public DashBoardAddListingModel(UserManager<ApplicationUser> userManager, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            UserManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            //Cities = new List<City>();

        }
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
        public async Task<IActionResult> OnGet()
        {
            if (staticCitiesList != null)
            {
                Cities = staticCitiesList;
            }
            
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/account/login");
            }
            return Page();
        }
        public IActionResult OnPostFillBranchesList([FromBody] List<Branch> branches)
        {
            Branches = new List<Branch>();
            foreach (var item in branches)
            {
                //Branch branch = new Branch { Title = item.Title };
                Branches.Add(item);
            }
            return new JsonResult(Branches);
        }
        //public IActionResult OnPostFillCountryList([FromBody] int val)
        //{
        //    staticCitiesList = new List<City>();
        //    Cities = _context.Cities.Where(e => e.CountryId == val).ToList();
        //    staticCitiesList = Cities;
        //    return new JsonResult(Cities);
        //}
        public async Task<IActionResult> OnPost(IFormFile Listinglogo, IFormFile PromoVideo, IFormFile listingbanner, IFormFileCollection Videos, IFormFileCollection Photos)
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/account/login");
            }

            if (AddListing.CategoryId == 0)
            {
                _toastNotification.AddErrorToastMessage("Select Category");
                return Page();
            }
            List<ListingPhotos> photolistings = new List<ListingPhotos>();
            if (Photos.Count() > 0)
            {
                for (int i = 0; i < Photos.Count(); i++)
                {
                    ListingPhotos photoobj = new ListingPhotos();
                    if (Photos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Photos/";
                        photoobj.PhotoUrl = UploadImage(folder, Photos[i]);
                    }

                    photolistings.Add(photoobj);
                }
                AddListing.ListingPhotos = photolistings;
            }


            List<ListingVideos> videoListing = new List<ListingVideos>();
            if (Videos.Count() > 0)
            {
                for (int i = 0; i < Videos.Count(); i++)
                {
                    ListingVideos videoobj = new ListingVideos();
                    if (Videos[i] != null)
                    {
                        string folder = "Images/ListingMedia/Videos/";
                        videoobj.VideoUrl = UploadImage(folder, Videos[i]);
                    }

                    videoListing.Add(videoobj);
                }
                AddListing.ListingVideos = videoListing;
            }

            if (Listinglogo != null)
            {
                string folder = "Images/ListingMedia/Logos/";
                AddListing.ListingLogo = UploadImage(folder, Listinglogo);
            }
            if (PromoVideo != null)
            {
                string folder = "Images/ListingMedia/Videos/";
                AddListing.PromoVideo = UploadImage(folder, PromoVideo);
            }
            if (listingbanner != null)
            {
                string folder = "Images/ListingMedia/Banners/";
                AddListing.ListingBanner = UploadImage(folder, listingbanner);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            AddListing.CreatedByUser = user.Email;
            AddListing.Branches = Branches;
            AddListing.AddedDate = DateTime.Now;
            _context.AddListings.Add(AddListing);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Listing Added successfully");



            return Redirect("/TemplatePages/DashBoardListingTable");

        }
    }
}
