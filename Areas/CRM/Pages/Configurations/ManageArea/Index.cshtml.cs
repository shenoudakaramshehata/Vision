using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;

namespace Vision.Areas.CRM.Pages.Configurations.ManageArea
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public string url { get; set; }
        [BindProperty]
        public Area AddArea { get; set; }
        public List<Area> Areas = new List<Area>();
        public List<City> Cities = new List<City>();
        public Area bannerObj { get; set; }

        public IRequestCultureFeature locale;
        public string BrowserCulture;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            AddArea = new Area();
            bannerObj = new Area();
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }
        public void OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            Areas = _context.Areas.Include(e => e.City).ToList();
            Cities = _context.Cities.ToList();
        }
        public async Task<IActionResult> OnGetSingleAreaForEdit(int AreaId)
        {
            AddArea = _context.Areas.Where(c => c.AreaId == AreaId).FirstOrDefault();

            return new JsonResult(AddArea);

        }
        public async Task<IActionResult> OnPostEditArea(int AreaId)
        {
            try
            {
                var model = _context.Areas.Where(c => c.AreaId == AreaId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("/CRM/Configurations/ManageArea/Index");
                    _toastNotification.AddSuccessToastMessage("Enter All Required Data");
                }



                model.AreaIsActive = AddArea.AreaIsActive;
                model.AreaOrderIndex = AddArea.AreaOrderIndex;
                model.AreaTlAr = AddArea.AreaTlAr;
                model.AreaTlEn = AddArea.AreaTlEn;
                model.CityId = AddArea.CityId;


                var UpdatedArea = _context.Areas.Attach(model);
                UpdatedArea.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Area Edited successfully");


            }
            catch (Exception e)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
                return RedirectToPage("/ManageArea/Index");
            }
            return Redirect("/CRM/Configurations/ManageArea/Index");

        }
        public IActionResult OnGetSingleAreaForView(int AreaId)
        {
            var Result = _context.Areas.Include(c => c.City).Where(c => c.AreaId == AreaId).Select(c => new
            {
                AreaId = c.AreaId,

                AreaIsActive = c.AreaIsActive,
                AreaOrderIndex = c.AreaOrderIndex,
                AreaTlAr = c.AreaTlAr,
                AreaTlEn = c.AreaTlEn,
                CityTlEn = c.City.CityTlEn,
                CityTlAr = c.City.CityTlAr,


            }).FirstOrDefault();
            return new JsonResult(Result);
        }

        public IActionResult OnGetSingleAreaForDelete(int AreaId)
        {
            AddArea = _context.Areas.Where(c => c.AreaId == AreaId).FirstOrDefault();
            return new JsonResult(AddArea);
        }

        public async Task<IActionResult> OnPostDeleteArea(int AreaId)
        {
            try
            {
                Area AreaObj = _context.Areas.Where(e => e.AreaId == AreaId).FirstOrDefault();


                if (AreaObj != null)
                {


                    _context.Areas.Remove(AreaObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Area Deleted successfully");

                }
                else
                    return Redirect("../Error");
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return Redirect("/CRM/Configurations/ManageArea/Index");
        }


        public async Task<IActionResult> OnPostAddArea()
        {

            try
            {
                _context.Areas.Add(AddArea);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Area Added Successfully");

            }
            catch (Exception e)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect("/CRM/Configurations/ManageArea/Index");

        }

    }
}
