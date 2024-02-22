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
    public class ViewClassifiedAdsModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public static bool ajax = true;
        private UserManager<ApplicationUser> _userManager { get; }
        public ViewClassifiedAdsModel(CRMDBContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;

        }

        public List<ClassifiedAd> ClassifiedAdsList { get; set; }
        public List<ClassifiedAdsCategory> TopAdsCategories { get; set; }
        public List<ClassifiedAdsCategory> SubAdsCategories { get; set; }

        public List<AdContentValue> AdContentValueList { get; set; }
        public static List<ClassifiedAd> ClassifiedAdsListStatic { get; set; }
        public static List<ClassifiedAd> classifiedaddsloc = new List<ClassifiedAd>();
        public static List<ClassifiedAd> Listings2 = new List<ClassifiedAd>();
        public static List<SortContentValueVm> adContentValues = new List<SortContentValueVm>();

        public int pages = 6;
        [BindProperty]
        public ClassifiedAdsFilterModel FilterModel { get; set; }
        public async Task<IActionResult> OnPostBranchesList(int x)
        {
            return new JsonResult(classifiedaddsloc);
        }
        public async Task<IActionResult> OnPostPagesList([FromBody] int num)
        {

            ajax = false;
            var start = (num - 1) * pages;
            ClassifiedAdsList = classifiedaddsloc.Skip(start).Take(pages).ToList();
            Listings2 = ClassifiedAdsList;
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
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Redirect("/identity/account/login");

            //}

            TopAdsCategories = _context.ClassifiedAdsCategories
                            .Where(a => a.ClassifiedAdsCategoryParentId == null)
                            .ToList();
            SubAdsCategories = new List<ClassifiedAdsCategory>();
            if (TopAdsCategories.Count > 0)
            {
                foreach (var category in TopAdsCategories)
                {
                    var categoryLv2 = _context.ClassifiedAdsCategories
                            .Where(a => a.ClassifiedAdsCategoryParentId == category.ClassifiedAdsCategoryId)
                            .ToList();
                    if(categoryLv2.Count > 0)
                    {
                        foreach(var cat in categoryLv2)
                        {
                            SubAdsCategories.Add(cat);

                        }
                    }
                }
            }

            try
            {
                int listCount = _context.ClassifiedAds.Count();

                
                //var alllist = await _context.ClassifiedAds.Where(a => a.IsActive == true).Include(a => a.ClassifiedAdsCategory).Include(a=>a.AdsImages).Take(pages).ToListAsync();
                Pagenumbers.Clear();
                Pagenumbers = getpagescount(listCount);
                if (first)
                {
                    var newalllist = await _context.ClassifiedAds.Where(a => a.IsActive == true).Include(a => a.ClassifiedAdsCategory).Include(a => a.AdsImages).ToListAsync();
                    classifiedaddsloc = newalllist;
                    ClassifiedAdsList = newalllist.Take(pages).ToList();
                    Listings2 = ClassifiedAdsList;
                    first = false;
                }
                else
                    ClassifiedAdsList = Listings2;
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            if (ClassifiedAdsListStatic != null)
            {
                if (ClassifiedAdsListStatic.Count != 0)
                {
                    Pagenumbers.Clear();
                    Pagenumbers = getpagescount(ClassifiedAdsListStatic.Count());
                    ClassifiedAdsList = ClassifiedAdsListStatic;
                    ClassifiedAdsList = ClassifiedAdsList.Take(pages).ToList();
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostClassifiedSortList([FromBody] List<string> SelectedValue)
        {
            ClassifiedAdsListStatic = new List<ClassifiedAd>();
            int restult = 0;
            bool checkValue = int.TryParse(SelectedValue[0], out restult);
            if (checkValue)
            {
                //var TempObjForPrice = await _context.AdTemplateConfigs.Where(e => e.FieldTypeId == 2 && e.AdTemplateFieldCaptionAr.Contains("السعر") && e.AdTemplateFieldCaptionEn.Contains("Price")).Select(e => e.AdTemplateConfigId).ToListAsync();
                //if (TempObjForPrice != null)
                //{
                //    var contentObj = await _context.AdContents.Where(e => TempObjForPrice.Contains(e.AdTemplateConfigId)).Select(e => e.AdContentId).ToListAsync();
                //    if (contentObj != null)
                //    {
                //        //var sortListOfContentValues =await _context.AdContentValues.Include(e=>e.AdContent).Where(e => contentObj.Contains(e.AdContentId))/*.OrderBy(e=> Convert.ToInt32(e.ContentValue))*/.Select(e=>e.AdContent.ClassifiedAdId).ToListAsync();
                //        var sortListOfContentValues = await _context.AdContentValues.Include(e => e.AdContent).Where(e => contentObj.Contains(e.AdContentId)).ToListAsync();

                //        if (sortListOfContentValues != null)
                //        {
                //            foreach (var item in sortListOfContentValues)
                //            {
                //                double Price = 0.0;
                //                bool checkRes = double.TryParse(item.ContentValue, out Price);
                //                if (checkRes)
                //                {
                //                    SortContentValueVm sortContentValueVm = new SortContentValueVm()
                //                    {
                //                        ClassifiedId = item.AdContent.ClassifiedAdId,
                //                        Price = Price
                //                    };
                //                    adContentValues.Add(sortContentValueVm);
                //                }


                //            }
                //        }
                //    }
                //}
                if (restult == 1)
                {
                    //var SortAdContentValues = adContentValues.OrderBy(e => e.Price).Select(e => e.ClassifiedId).ToList();
                    //foreach (var item in SortAdContentValues)
                    //{
                    //    var classified = await _context.ClassifiedAds.Where(a => a.ClassifiedAdId == item && a.IsActive == true).Include(a => a.ClassifiedAdsCategory).ThenInclude(a => a.AdTemplateConfigs).FirstOrDefaultAsync();
                    //    ClassifiedAdsListStatic.Add(classified);

                    //}
                    ////ClassifiedAdsListStatic = await _context.ClassifiedAds.Where(a => SortAdContentValues.Contains(a.ClassifiedAdId)&& a.IsActive == true).Include(a => a.ClassifiedAdsCategory).ThenInclude(a => a.AdTemplateConfigs).ToListAsync(); ;
                    //classifiedaddsloc = ClassifiedAdsListStatic;
                    ClassifiedAdsListStatic = classifiedaddsloc.OrderBy(e => e.Price).ToList();
                    classifiedaddsloc = ClassifiedAdsListStatic;
                }

                if (restult == 2)
                {
                    //var SortAdContentValues = adContentValues.OrderByDescending(e => e.Price).Select(e => e.ClassifiedId).ToList();
                    //foreach (var item in SortAdContentValues)
                    //{
                    //    var classified = await _context.ClassifiedAds.Where(a => a.ClassifiedAdId == item && a.IsActive == true).Include(a => a.ClassifiedAdsCategory).ThenInclude(a => a.AdTemplateConfigs).FirstOrDefaultAsync();
                    //    ClassifiedAdsListStatic.Add(classified);

                    //}
                    //classifiedaddsloc = ClassifiedAdsListStatic;
                    ClassifiedAdsListStatic = classifiedaddsloc.OrderByDescending(e => e.Price).ToList();
                    classifiedaddsloc = ClassifiedAdsListStatic;
                }
            }
            //Pagenumbers = getpagescount(ClassifiedAdsListStatic);
            return new JsonResult(SelectedValue);

        }
        
        public async Task<ActionResult> OnPostFilterClassifiedAd([FromBody] ClassifiedFilterVm classifiedAdsVM)
        {
            List<long> ClassifiedIds = new List<long>();
            try
            {
                var classifiedAdsList = await _context.ClassifiedAds.Where(a => a.ClassifiedAdsCategoryId == classifiedAdsVM.ClassifiedAdsCategoryId && a.IsActive == true).Include(a => a.ClassifiedAdsCategory).ThenInclude(a => a.AdTemplateConfigs).ToListAsync();
                if (classifiedAdsVM.addContentVMs != null)
                {
                    foreach (var item in classifiedAdsVM.addContentVMs)
                    {
                        var Values = await _context.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && e.ContentValue == item.Values[0]).Select(e => e.ContentValue).ToListAsync();
                        ClassifiedIds = _context.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
                        classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

                    }
                    
                }
                ClassifiedAdsListStatic= classifiedAdsList;
                classifiedaddsloc = ClassifiedAdsListStatic;
                
            }

            catch (Exception)
            {
           
            }
            return new JsonResult(true);
        }
        public async Task<IActionResult> OnPostFavourite([FromBody] int Classifiedid)
        {
            bool favouriteflag = false;
            var user = await _userManager.GetUserAsync(User);
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
