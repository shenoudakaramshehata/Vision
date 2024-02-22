using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.ViewModel;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.ViewModels;

namespace Vision.Pages
{

    public class AddBDOfferModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public BDOfferVm AddBDOfferObj { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public static long bussinessId = 0;

        public AddBDOfferModel(UserManager<ApplicationUser> userManager, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            UserManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            

        }
        public void OnGet(long BDId)
        {
            bussinessId = BDId;
        }
        public async Task<IActionResult> OnPost(IFormFile pic, IFormFileCollection Photos)
        {
            try
            {
                bool SubscriptionIsFinished = _context.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == bussinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault() == null ? true : _context.BusiniessSubscriptions.Where(e => e.ClassifiedBusinessId == bussinessId && e.IsActive == true).OrderByDescending(e => e.BusiniessSubscriptionId).FirstOrDefault().EndDate < DateTime.Now ? true : false;
                //if (SubscriptionIsFinished)
                //{
                //    _toastNotification.AddErrorToastMessage("You Must Subscribe...Firstly");
                //    return Page();

                //}
                var BDOffer = new BDOffer()
                {
                    ClassifiedBusinessId = bussinessId,
                    TitleAr = AddBDOfferObj.TitleAr,
                    TitleEn = AddBDOfferObj.TitleEn,
                    Price = AddBDOfferObj.Price,
                    OfferDescription = AddBDOfferObj.OfferDescription,
                    PublishDate = DateTime.Now,
                };
                BDOffer.Pic = UploadImage("Images/BDOffer/", pic);

                if (Photos != null)
                {
                    List<BDOfferImage> BDOfferImages = new List<BDOfferImage>();
                    foreach (var item in Photos)
                    {
                        var BDOfferImageObj = new BDOfferImage();
                        BDOfferImageObj.Image = UploadImage("Images/BDOffer/", item);
                        BDOfferImages.Add(BDOfferImageObj);

                    }
                    BDOffer.BDOfferImages = BDOfferImages;

                }
                _context.BDOffers.Add(BDOffer);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Offer Added Successfully");
            }
            catch(Exception)
            {
                _toastNotification.AddErrorToastMessage("SomeThing Went Error");
               
            }
            return Redirect($"/BDOffers?BDId={bussinessId}");
        }
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
