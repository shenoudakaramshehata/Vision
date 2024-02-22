using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using Vision.Data;
using Vision.Models;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Vision.ViewModels;
using NToastNotify;
using System.Globalization;
using Vision.Migrations.CRMDB;

namespace Vision.Pages
{
    public class StoreListModel : PageModel
    {
        private readonly CRMDBContext _context;


        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public readonly IToastNotification toastNotification;
        public HttpClient httpClient { get; set; }
       
        private readonly UserManager<ApplicationUser> _userManager;
        public IRequestCultureFeature locale;
        public string BrowserCulture;

        public List<BusinessCategory> BusinessCategories { get; set; }
        public List<City> Cities { get; set; }
        public List<Area> Areas { get; set; }
        public List<ClassifiedBusiness> ClassifiedBusiness { get; set; }
        public StoreListModel(IToastNotification _toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IRazorPartialToStringRenderer renderer, CRMDBContext Context, ApplicationDbContext dbContext)
        {
            _context = Context;
            _dbContext = dbContext;
            toastNotification = _toastNotification;
            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            BusinessCategories = new List<BusinessCategory>();

        }

        public async Task<IActionResult> OnGet()
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            BusinessCategories = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == null).ToList();
            Cities = _context.Cities.Where(e => e.CityIsActive == true).ToList();
            Areas= _context.Areas.Where(e => e.AreaIsActive == true).ToList();
            ClassifiedBusiness = _context.ClassifiedBusiness.Where(e => e.IsActive == true).ToList();
            return Page();

        }
        public IActionResult OnGetChildCategory(int parseselectedValue) {
        var childCatgories= _context.BusinessCategories.Where(e=>e.BusinessCategoryParentId== parseselectedValue).Select(
                 i => new
                 {


                     BusinessCategoryId = i.BusinessCategoryId,
                     BusinessCategoryTitleAr = i.BusinessCategoryTitleAr,
                     BusinessCategoryTitleEn = i.BusinessCategoryTitleEn
                    
                 }).ToList();
            return new JsonResult(childCatgories);

        }
        public IActionResult OnGetBusinessList(int parseselectedValue, int parseCityselectedValue , int parseAreaselectedValue)
        {
            var Business = _context.ClassifiedBusiness.Where(e => e.BusinessCategoryId == parseselectedValue&&e.AreaId== parseAreaselectedValue && e.CityId == parseCityselectedValue && e.IsActive == true).Select(
                                 i => new
                                 {


                                     Mainpic = i.Mainpic,
                                     Logo = i.Logo,
                                     Title = i.Title,
                                     Rating = i.Rating,
                                     Address = i.Address,
                                     phone = i.phone,
                                     Email = i.Email,

                                 }).ToList();
            return new JsonResult(Business);
        }
        //public IActionResult OnPost()
        //{
        //    var childCategory = Request.Form["childCategory"];
        //    var Area = Request.Form["Area"];
        //    var City = Request.Form["City"];
        //    return Page();
        //}

    }
}
