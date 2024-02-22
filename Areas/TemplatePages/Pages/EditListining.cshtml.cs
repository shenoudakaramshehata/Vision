using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Areas.TemplatePages.Pages
{
    public class EditListiningModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public EditListiningModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        [BindProperty]
        public AddListing AddListing { get; set; }
        public static List<Branch> Branches;
        public List<Branch> BranchesList = new List<Branch>();
        public static int listobjid { get; set; }
        public IActionResult OnPostFillBranchesList([FromBody] List<string> branchTitle)
        {
            Branches = new List<Branch>();
            foreach (var item in branchTitle)
            {
                Branch branch = new Branch { Title = item };
                Branches.Add(branch);
            }
            return new JsonResult(Branches);
        }
        public IActionResult OnPostDeletePhotoById([FromBody] int PhotoId)
        {
            try
            {
                var photoobj = _context.ListingPhotos.Where(a => a.Id == PhotoId).FirstOrDefault();
                if (photoobj == null)
                {
                    return Page();
                }
                _context.ListingPhotos.Remove(photoobj);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
            return new JsonResult(PhotoId);

        }
        public IActionResult OnPostDeleteVideoById([FromBody] int Videoid)
        {
            try
            {
                var Videoobj = _context.ListingVideos.Where(a => a.Id == Videoid).FirstOrDefault();
                if (Videoobj == null)
                {
                    return Page();
                }
                _context.ListingVideos.Remove(Videoobj);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
            return new JsonResult(Videoid);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                listobjid = id;
                AddListing = await _context.AddListings.Include(a => a.Branches).Include(a => a.ListingPhotos).Include(a => a.ListingVideos).FirstOrDefaultAsync(m => m.AddListingId == id);
                if (AddListing == null)
                {
                    return Redirect("../Error");
                }
                BranchesList = AddListing.Branches.ToList();
                Branches = AddListing.Branches.ToList();
                return Page();

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Page();
        }
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
        public async Task<IActionResult> OnPostAsync(IFormFile Listinglogo, IFormFile PromoVideo, IFormFile listingbanner, IFormFileCollection Videos, IFormFileCollection Photos)
        {
            var model = _context.AddListings.Where(c => c.AddListingId == AddListing.AddListingId).FirstOrDefault();
            if (Listinglogo != null)
            {
                string folder = "Images/ListingMedia/Logos/";
                AddListing.ListingLogo = UploadImage(folder, Listinglogo);
                model.ListingLogo = AddListing.ListingLogo;
            }
            if (PromoVideo != null)
            {
                string folder = "Images/ListingMedia/Videos/";
                AddListing.PromoVideo = UploadImage(folder, PromoVideo);
                model.PromoVideo = AddListing.PromoVideo;
            }
            if (listingbanner != null)
            {
                string folder = "Images/ListingMedia/Banners/";
                AddListing.ListingBanner = UploadImage(folder, listingbanner);
                model.ListingBanner = AddListing.ListingBanner;
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
                model.ListingPhotos = photolistings;
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
                model.ListingVideos = videoListing;

            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (model == null)
                {
                    return Page();
                }
                var branchesid = _context.Branches.Where(a => a.AddListingId == AddListing.AddListingId);
                _context.Branches.RemoveRange(branchesid);
                model.Address = AddListing.Address;
                model.Title = AddListing.Title;
                model.Phone1 = AddListing.Phone1;
                model.Phone2 = AddListing.Phone2;
                model.Tags = AddListing.Tags;
                model.Reviews = AddListing.Reviews;
                model.Rating = AddListing.Rating;
                model.MainLocataion = AddListing.MainLocataion;
                model.CityId = AddListing.CityId;
                model.ContactPeroson = AddListing.ContactPeroson;
                model.Country = AddListing.Country;
                model.Branches = Branches;
                model.CategoryId = AddListing.CategoryId;
                model.Discription = AddListing.Discription;
                model.Email = AddListing.Email;
                model.Fax = AddListing.Fax;
                model.Website = AddListing.Website;

                _context.Attach(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Listining Edited successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return RedirectToPage("./dashboardlistingtable");
        }
    }
}
