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
using Microsoft.EntityFrameworkCore;

namespace Vision.Pages
{
    public class EditBDOfferModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public BDOffer EditBDOfferObj { get; set; }
        public UserManager<ApplicationUser> UserManager { get; }
        public static int BDOfferId = 0;
        public static long BDId = 0;

        public EditBDOfferModel(UserManager<ApplicationUser> userManager, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            UserManager = userManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;


        }
        public IActionResult OnPostDeletePhotoById([FromBody] int PhotoId)
        {
            try
            {
                var photoobj = _context.BDOfferImages.Where(a => a.BDOfferImageId == PhotoId).FirstOrDefault();
                if (photoobj == null)
                {
                    return Page();
                }
                _context.BDOfferImages.Remove(photoobj);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error..");
                return Page();
            }
            return new JsonResult(PhotoId);

        }
        public IActionResult OnGet(int Id)
        {
            try
            {
                EditBDOfferObj = _context.BDOffers.Include(e=>e.BDOfferImages).Where(e => e.BDOfferId == Id).FirstOrDefault();
                if (EditBDOfferObj == null)
                {
                    return Redirect("/PageNF");
                }
                BDOfferId = Id;
                BDId = EditBDOfferObj.ClassifiedBusinessId;
            }
            catch(Exception)
            {
                return Redirect("/PageNF");
            }
            return Page();
           
        }

        public async Task<IActionResult> OnPost(IFormFile pic, IFormFileCollection Photos)
        {
            try
            {
                var BDOfferObj = _context.BDOffers.Where(e => e.BDOfferId == BDOfferId).FirstOrDefault();
                if (BDOfferObj == null)
                {
                    return Redirect("/PageNF");

                }
                BDOfferObj.Price = EditBDOfferObj.Price;
                BDOfferObj.TitleAr = EditBDOfferObj.TitleAr;
                BDOfferObj.TitleEn = EditBDOfferObj.TitleEn;
                BDOfferObj.OfferDescription = EditBDOfferObj.OfferDescription;


                if (pic != null)
                {
                    BDOfferObj.Pic = UploadImage("Images/BDOffer/", pic);
                }
                _context.Attach(BDOfferObj).State = EntityState.Modified;
                if (Photos != null)
                {
                    List<BDOfferImage> BDOfferImages = new List<BDOfferImage>();
                    foreach (var item in Photos)
                    {
                        var BDOfferImageObj = new BDOfferImage();
                        BDOfferImageObj.Image = UploadImage("Images/BDOffer/", item);
                        BDOfferImageObj.BDOfferId = BDOfferObj.BDOfferId;
                        BDOfferImages.Add(BDOfferImageObj);

                    }
                    _context.BDOfferImages.AddRange(BDOfferImages);

                }

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Offer Edited Successfully");
               



            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("SomeThing Went Error");
                
            }
            return Redirect($"/BDOffers?BDId={BDId}");
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
