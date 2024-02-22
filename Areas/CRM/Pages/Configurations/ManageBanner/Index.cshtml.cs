using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;

namespace Vision.Areas.CRM.Pages.Configurations.ManageBanner
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
       
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public string url { get; set; }
        [BindProperty]
        public Banner AddBanner { get; set; }
        public List<Banner> Banners = new List<Banner>();
        public List<EntityType> EntityTypes = new List<EntityType>();
        public Banner bannerObj { get; set; }

        public IRequestCultureFeature locale;
        public string BrowserCulture;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager,ApplicationDbContext applicationDbContext, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            AddBanner = new Banner();
            bannerObj = new Banner();
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public void OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            Banners = _context.Banners.Include(e=>e.EntityType).ToList();
            EntityTypes = _context.EntityTypes.ToList();
        }
        public async Task<IActionResult> OnGetSingleBannerForEdit(int BannerId)
        {
            AddBanner = _context.Banners.Where(c => c.BannerId == BannerId).FirstOrDefault();
            string EntityName = " ";
           
            if (AddBanner.EntityTypeId == 1)
            {
                long EId = 0;
                bool checkRes = long.TryParse(AddBanner.EntityId, out EId);
                if (checkRes)
                {
                    EntityName = _context.ClassifiedAds.Where(e => e.ClassifiedAdId == EId).FirstOrDefault().TitleAr;


                }



            }
            if (AddBanner.EntityTypeId == 2)
            {
                long EId = 0;
                bool checkRes = long.TryParse(AddBanner.EntityId, out EId);
                if (checkRes)
                {
                    EntityName = _context.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == EId).FirstOrDefault().Title;
                }




            }
            if (AddBanner.EntityTypeId == 3)
            {
               // EntityName = await _userManager.Users.Where(e => e.Id == AddBanner.EntityId).FirstOrDefault().FullName;
             
            }
            var EditBannerVm = new EditBannerVm()
            {
                BannerId=AddBanner.BannerId,
                BannerIsActive = AddBanner.BannerIsActive,
                BannerOrderIndex = AddBanner.BannerOrderIndex,
                BannerPic = AddBanner.BannerPic,
                LargePic = AddBanner.LargePic,
                EntityId = AddBanner.EntityId,
                EntityTypeId = AddBanner.EntityTypeId,
                EntityName=EntityName,
                
            };

            return new JsonResult(EditBannerVm);

        }
        public async Task<IActionResult> OnPostEditBanner(int BannerId,IFormFile EditLargeImage, IFormFile Editfile, string EditSelectedVal)
        {
            try
            {
                var model = _context.Banners.Where(c => c.BannerId == BannerId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("/CRM/Configurations/ManageBanner/Index");
                }


                if (Editfile != null)
                {
                    if (model.BannerPic != null)
                    {
                        var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.BannerPic);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }

                    string folder = "Images/Banner/";
                    model.BannerPic = await UploadImage(folder, Editfile);
                }
                else
                {
                    model.BannerPic = AddBanner.BannerPic;
                }

                if (EditLargeImage != null)
                {
                    if (model.LargePic != null)
                    {
                        var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.LargePic);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }

                    string folder = "Images/Banner/";
                    model.LargePic = await UploadImage(folder, EditLargeImage);
                }
                else
                {
                    model.LargePic = AddBanner.LargePic;
                }
                model.EntityTypeId = AddBanner.EntityTypeId;
                model.BannerOrderIndex = AddBanner.BannerOrderIndex;
                model.BannerIsActive = AddBanner.BannerIsActive;
                model.EntityId = EditSelectedVal;
                
                var UpdatedBanner = _context.Banners.Attach(model);
                UpdatedBanner.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Banner Edited successfully");


            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
                return RedirectToPage("/Banner/Index");
            }
            return Redirect("/CRM/Configurations/ManageBanner/Index");

        }
        public IActionResult OnGetSingleBannerForView(int BannerId)
        {
            var Result = _context.Banners.Include(c=>c.EntityType).Where(c => c.BannerId == BannerId).Select(c=>new
            {
                BannerId=c.BannerId,
                
                BannerIsActive=c.BannerIsActive,
                BannerOrderIndex=c.BannerOrderIndex,
                BannerPic=c.BannerPic,
                EntityId=c.EntityId,
                EntityTitleEn=c.EntityType.EntityTitleEn,
                EntityTypeId = c.EntityTypeId,
                LargePic = c.LargePic,

                
            }).FirstOrDefault();
            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnGetSingleFillData(int BannerTypeId)
        {
            if (BannerTypeId==1)
            {
                var Result = _context.ClassifiedAds.Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,



                }).ToList();
                return new JsonResult(Result);
                


            }
            if (BannerTypeId == 2)
            {
                var Result = _context.ClassifiedBusiness.Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    Title = c.Title,

                }).ToList();
                return new JsonResult(Result);



            }
            if (BannerTypeId == 3)
            {
                var Result =await _userManager.Users.Select(c => new
                {
                    Id = c.Id,
                    FullName = c.FullName,

                }).ToListAsync();
                return new JsonResult(Result);
            }
            return new JsonResult(true);
        }
        public IActionResult OnGetSingleBannerForDelete(int BannerId)
        {
            AddBanner = _context.Banners.Where(c => c.BannerId == BannerId).FirstOrDefault();
            return new JsonResult(AddBanner);
        }

        public async Task<IActionResult> OnPostDeleteBanner(int BannerId)
        {
            try
            {
                Banner bannerObj = _context.Banners.Where(e => e.BannerId == BannerId).FirstOrDefault();


                if (bannerObj != null)
                {


                    _context.Banners.Remove(bannerObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Banner Deleted successfully");
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Banner/" + AddBanner.BannerPic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                }
                else
                    return Redirect("../Error");
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return Redirect("/CRM/Configurations/ManageBanner/Index");
        }


        public async Task<IActionResult> OnPostAddBanner(IFormFile file, IFormFile LargeImage,string SelectedVal)
        {
            
            try
            {
                if (file != null)
                {
                    string folder = "Images/Banner/";
                    AddBanner.BannerPic = await UploadImage(folder, file);
                }
                if (LargeImage != null)
                {
                    string folder = "Images/Banner/";
                    AddBanner.LargePic = await UploadImage(folder, LargeImage);
                }
                AddBanner.EntityId = SelectedVal;
                _context.Banners.Add(AddBanner);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Banner Added Successfully");

            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/CRM/Configurations/ManageBanner/Index");
            
        }
        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
