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
using Vision.ViewModels;

namespace Vision.Pages
{
    public class ViewBussinessDirectoryModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public static bool ajax = true;
        private UserManager<ApplicationUser> _userManager { get; }
        public ViewBussinessDirectoryModel(CRMDBContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;

        }

        public List<ClassifiedBusiness> BussinessList { get; set; }
        public List<BusinessContentValue> AdContentValueList { get; set; }
        public static List<ClassifiedBusiness> BusinessListStatic { get; set; }
        public static List<ClassifiedBusiness> Businessaddsloc = new List<ClassifiedBusiness>();
        public static List<ClassifiedBusiness> Listings2 = new List<ClassifiedBusiness>();
        public static List<SortContentValueVm> adContentValues = new List<SortContentValueVm>();

        public int pages = 6;
        [BindProperty]
        public ClassifiedAdsFilterModel FilterModel { get; set; }
        public async Task<IActionResult> OnPostBranchesList(int x)
        {
            return new JsonResult(Businessaddsloc);
        }
        public async Task<IActionResult> OnPostPagesList([FromBody] int num)
        {

            ajax = false;
            var start = (num - 1) * pages;
            BussinessList = Businessaddsloc.Skip(start).Take(pages).ToList();
            Listings2 = BussinessList;
            return new JsonResult(num);
        }
        public List<int> getpagescount(int count)
        {

            float number = count / pages;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Pagenumbers;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/account/login");

            }

            try
            {
                int listCount = _context.ClassifiedBusiness.Count();


                var alllist = await _context.ClassifiedBusiness.Where(a => a.IsActive == true).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).Take(pages).ToListAsync();
                Pagenumbers.Clear();
                Pagenumbers = getpagescount(listCount);
                if (first)
                {
                    var newalllist = await _context.ClassifiedBusiness.Where(a => a.IsActive == true).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).ToListAsync();
                    Businessaddsloc = newalllist;
                    BussinessList = newalllist.Take(pages).ToList();
                    Listings2 = BussinessList;
                    first = false;
                }
                else
                    BussinessList = Listings2;
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            if (BusinessListStatic != null)
            {
                if (BusinessListStatic.Count != 0)
                {
                    Pagenumbers.Clear();
                    Pagenumbers = getpagescount(BusinessListStatic.Count());
                    BussinessList = BusinessListStatic;
                    BussinessList = BussinessList.Take(pages).ToList();
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostClassifiedSortList([FromBody] List<string> SelectedValue)
        {
            
            int restult = 0;
            bool checkValue = int.TryParse(SelectedValue[0], out restult);
            if (checkValue)
            {
                var BusinessListStatic2 = await _context.ClassifiedBusiness.Where(a => a.IsActive == true).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).ToListAsync();
                if (restult == 1)
                {
                    BusinessListStatic=BusinessListStatic2.OrderBy(e => e.Rating).ToList();
                    Businessaddsloc = BusinessListStatic;
                }

                if (restult == 2)
                {
                    BusinessListStatic=BusinessListStatic2.OrderByDescending(e => e.Rating).ToList();
                    Businessaddsloc = BusinessListStatic;

                }
            }
            
            return new JsonResult(SelectedValue);

        }

        public async Task<ActionResult> OnPostFilterBussinessDirectory([FromBody] BussinessFilterVm bussinessDVM)
        {
            List<long> BussinessDIds = new List<long>();
            try
            {
                var BDList =  await _context.ClassifiedBusiness.Where(a => a.IsActive == true&a.BusinessCategoryId== bussinessDVM.BussinessCategoryId).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).ToListAsync();
                if (bussinessDVM.addContentVMs != null)
                {
                    foreach (var item in bussinessDVM.addContentVMs)
                    {
                        var Values = await _context.BusinessContentValues.Include(e => e.BusinessContent).Where(e => e.BusinessContent.BusinessTemplateConfigId == item.BusinessTemplateConfigId && e.ContentValue == item.Values[0]).Select(e => e.ContentValue).ToListAsync();
                        BussinessDIds = _context.BusinessContents.Include(e => e.BusinessContentValues).Where(e => Values.Contains(e.BusinessContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedBusinessId).ToList();
                        BDList = BDList.Where(e => BussinessDIds.Contains(e.ClassifiedBusinessId)).ToList();

                    }

                }
                BusinessListStatic = BDList;
                Businessaddsloc = BusinessListStatic;

            }

            catch (Exception)
            {

            }
            return new JsonResult(true);
        }
        public IActionResult OnGetInfoList(int CatId)
        {
            var Result = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == CatId).OrderBy(e => e.SortOrder).Select(c => new
            {
                ClassifiedAdsCategoryId = c.BusinessCategoryId,
                AdTemplateFieldCaptionAr = c.BusinessTemplateFieldCaptionAr,
                AdTemplateFieldCaptionEn = c.BusinessTemplateFieldCaptionEn,
                FieldTypeId = c.FieldTypeId,
                AdTemplateConfigId = c.BusinessTemplateConfigId,
                IsRequired = c.IsRequired,
                ValidationMessageAr = c.ValidationMessageAr,
                ValidationMessageEn = c.ValidationMessageEn,
                AdTemplateOptions = c.BusinessTemplateOptions
            }).ToList();


            return new JsonResult(Result);
        }
        public async Task<IActionResult> OnPostFavourite([FromBody] int BDid)
        {
            bool favouriteflag = false;
            var user = await _userManager.GetUserAsync(User);
            var favourite = _context.FavouriteBusiness.Where(a => a.UserId == user.Id && a.ClassifiedBusinessId == BDid).FirstOrDefault();
            if (favourite == null)
            {
                var favouriteobj = new FavouriteBusiness() { ClassifiedBusinessId = BDid, UserId = user.Id };

                _context.FavouriteBusiness.Add(favouriteobj);
                favouriteflag = true;
            }
            else
                _context.FavouriteBusiness.Remove(favourite);
            _context.SaveChanges();
            return new JsonResult(favouriteflag);
        }

    }
}