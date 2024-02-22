using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;

namespace Vision.Areas.CRM.Pages.Configurations.ManageSlider
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public string url { get; set; }
        [BindProperty]
        public Slider AddSlider { get; set; }
        public List<Slider> Sliders = new List<Slider>();
        public List<EntityType> EntityTypes = new List<EntityType>();
        public Slider SliderObj { get; set; }

        public IRequestCultureFeature locale;
        public string BrowserCulture;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            AddSlider = new Slider();
            SliderObj = new Slider();
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public void OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            Sliders = _context.Sliders.Include(e => e.EntityType).ToList();
            EntityTypes = _context.EntityTypes.ToList();
        }
        public async Task<IActionResult> OnGetSingleSliderForEdit(int SliderId)
        {
            AddSlider = _context.Sliders.Where(c => c.SliderId == SliderId).FirstOrDefault();
            string EntityName = " ";

            if (AddSlider.EntityTypeId == 1)
            {
                long EId = 0;
                bool checkRes = long.TryParse(AddSlider.EntityId, out EId);
                if (checkRes)
                {
                    EntityName = _context.ClassifiedAds.Where(e => e.ClassifiedAdId == EId).FirstOrDefault().TitleAr;


                }



            }
            if (AddSlider.EntityTypeId == 2)
            {
                long EId = 0;
                bool checkRes = long.TryParse(AddSlider.EntityId, out EId);
                if (checkRes)
                {
                    EntityName = _context.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == EId).FirstOrDefault().Title;
                }




            }
            if (AddSlider.EntityTypeId == 3)
            {
                // EntityName = await _userManager.Users.Where(e => e.Id == AddBanner.EntityId).FirstOrDefault().FullName;

            }
            var EditSliderVm = new EditSliderVm()
            {
                SliderId = AddSlider.SliderId,
                IsActive = AddSlider.IsActive,
                OrderIndex = AddSlider.OrderIndex,
                PageIndex = AddSlider.PageIndex,
                Pic = AddSlider.Pic,
                EntityId = AddSlider.EntityId,
                EntityTypeId = AddSlider.EntityTypeId,
                EntityName = EntityName,

            };

            return new JsonResult(EditSliderVm);

        }
        public async Task<IActionResult> OnPostEditSlider(int SliderId, IFormFile Editfile, string EditSelectedVal)
        {
            try
            {
                var model = _context.Sliders.Where(c => c.SliderId == SliderId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("/CRM/Configurations/ManageSlider/Index");
                }


                if (Editfile != null)
                {
                    if (model.Pic != null)
                    {
                        var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, model.Pic);
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }

                    string folder = "Images/Slider/";
                    model.Pic = await UploadImage(folder, Editfile);
                }
                else
                {
                    model.Pic = AddSlider.Pic;
                }

               
                model.EntityTypeId = AddSlider.EntityTypeId;
                model.OrderIndex = AddSlider.OrderIndex;
                model.IsActive = AddSlider.IsActive;
                model.PageIndex = AddSlider.PageIndex;
                model.EntityId = EditSelectedVal;

                var UpdatedSlider = _context.Sliders.Attach(model);
                UpdatedSlider.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Slider Edited successfully");


            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
                
            }
            return Redirect("/CRM/Configurations/ManageSlider/Index");

        }
        public IActionResult OnGetSingleSliderForView(int SliderId)
        {
            var Result = _context.Sliders.Include(c => c.EntityType).Where(c => c.SliderId == SliderId).Select(c => new
            {
                SliderId = c.SliderId,

                IsActive = c.IsActive,
                OrderIndex = c.OrderIndex,
                Pic = c.Pic,
                EntityId = c.EntityId,
                EntityTitleEn = c.EntityType.EntityTitleEn,
                EntityTypeId = c.EntityTypeId,
                PageIndex = c.PageIndex,


            }).FirstOrDefault();
            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnGetSingleFillData(int SliderTypeId)
        {
            if (SliderTypeId == 1)
            {
                var Result = _context.ClassifiedAds.Select(c => new
                {
                    ClassifiedAdId = c.ClassifiedAdId,
                    TitleAr = c.TitleAr,
                    TitleEn = c.TitleEn,



                }).ToList();
                return new JsonResult(Result);



            }
            if (SliderTypeId == 2)
            {
                var Result = _context.ClassifiedBusiness.Select(c => new
                {
                    ClassifiedBusinessId = c.ClassifiedBusinessId,
                    Title = c.Title,

                }).ToList();
                return new JsonResult(Result);



            }
            if (SliderTypeId == 3)
            {
                var Result = await _userManager.Users.Select(c => new
                {
                    Id = c.Id,
                    FullName = c.FullName,

                }).ToListAsync();
                return new JsonResult(Result);
            }
            return new JsonResult(true);
        }
        public IActionResult OnGetSingleSliderForDelete(int SliderId)
        {
            AddSlider = _context.Sliders.Where(c => c.SliderId == SliderId).FirstOrDefault();
            return new JsonResult(AddSlider);
        }

        public async Task<IActionResult> OnPostDeleteSlider(int SliderId)
        {
            try
            {
                Slider sliderObj = _context.Sliders.Where(e => e.SliderId == SliderId).FirstOrDefault();


                if (sliderObj != null)
                {


                    _context.Sliders.Remove(sliderObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Slider Deleted successfully");
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Slider/" + AddSlider.Pic);
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

            return Redirect("/CRM/Configurations/ManageSlider/Index");
        }


        public async Task<IActionResult> OnPostAddSlider(IFormFile file, string SelectedVal)
        {

            try
            {
                if (file != null)
                {
                    string folder = "Images/Slider/";
                    AddSlider.Pic = await UploadImage(folder, file);
                }

                AddSlider.EntityId = SelectedVal;
                _context.Sliders.Add(AddSlider);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Slider Added Successfully");

            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/CRM/Configurations/ManageSlider/Index");

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
