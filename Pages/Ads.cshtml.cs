using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Services;
using Vision.ViewModels;
using Newtonsoft.Json;
using System.Drawing.Printing;
using NuGet.Packaging.Signing;

namespace Vision.Pages
{
    public class AdsModel : PageModel
    {
        private CRMDBContext _context;
        public List<ClassifiedAd> ClassifiedAds = new List<ClassifiedAd>();
        public List<ClassifiedAdsCategory> ClassifiedAdsCategory = new List<ClassifiedAdsCategory>();
        public List<City> Cities { get; set; }
        private readonly IRazorPartialToStringRenderer _renderer;
        public string FormTemplate { get; set; }
        private UserManager<ApplicationUser> _userManager { get; }
        private readonly IWebHostEnvironment _hostEnvironment;
        private IToastNotification _toastNotification { get; }
        public static List<ClassifiedAd> Ads = new List<ClassifiedAd>();
        public List<ClassifiedAd> AdsList { get; set; }
        public int catId { get; set; }
        public AdsModel(IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _renderer = renderer;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _context = Context;
            _toastNotification = toastNotification;
            AdsList = new List<ClassifiedAd>();
        }

        public async Task<IActionResult> OnGet(int catid, int? selectedValue)
        {
            var selectedValue1 = Request.Query["controlValue"];
            ClassifiedAdsCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == catid).ToList();
            if (ClassifiedAdsCategory.Count == 0)
            {
                var cat = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == catid).FirstOrDefault();
                ClassifiedAdsCategory.Add(cat);
            }
            Cities = _context.Cities.Where(e => e.CityIsActive == true).ToList();
            catId = catid;
            var catList = _context.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == catid).Select(e => e.SearchCatagoryLevel).ToList();
            ClassifiedAds = _context.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => catList.Contains(e.ClassifiedAdsCategoryId.Value)).ToList();
            if (Ads.Count != 0)
            {
                ClassifiedAds = Ads;

                //}
                return Page();
            }
            return Page();
        }
            public IActionResult OnGetArea(int parseselectedValue)
            {
                var area = _context.Areas.Where(e => e.CityId == parseselectedValue).Select(
                         i => new
                         {


                             AreaId = i.AreaId,
                             AreaTlAr = i.AreaTlAr,
                             AreaTlEn = i.AreaTlEn

                         }).ToList();
                return new JsonResult(area);

            }


            public IActionResult OnGetInfoList(int CatId)
            {
                var Result = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == 857).OrderBy(e => e.SortOrder).Select(c => new
                {
                    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                    AdTemplateFieldCaptionAr = c.AdTemplateFieldCaptionAr,
                    AdTemplateFieldCaptionEn = c.AdTemplateFieldCaptionEn,
                    FieldTypeId = c.FieldTypeId,
                    AdTemplateConfigId = c.AdTemplateConfigId,
                    IsRequired = c.IsRequired,
                    ValidationMessageAr = c.ValidationMessageAr,
                    ValidationMessageEn = c.ValidationMessageEn,
                    AdTemplateOptions = c.AdTemplateOptions
                }).ToList();


                return new JsonResult(Result);
            }

            public IActionResult OnPostFilterAds(string parseFilterArray, int catId)
            {
                List<NewFilterAds> newFilterAds = JsonConvert.DeserializeObject<List<NewFilterAds>>(parseFilterArray);

                newFilterAds.RemoveAll(filter => filter.value.Count == 0);

                try
                {
                    var classifiedCatagory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == catId).FirstOrDefault();
                    if (classifiedCatagory == null)
                    {
                        return Page();
                    }

                    List<long> ClassifiedIds = new List<long>();
                    var classifiedAdsList = _context.ClassifiedAds.Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(a => a.ClassifiedAdsCategoryId == catId).ToList();

                    if (classifiedAdsList != null)
                    {

                        if (newFilterAds != null)
                        {
                            foreach (var item in newFilterAds)
                            {
                                if (item.AdTemplateConfigId != 0)
                                {
                                    var Values = _context.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && item.value.Contains(e.ContentValue)).Select(e => e.ContentValue).ToList();
                                    ClassifiedIds = _context.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
                                    classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

                                }

                            }
                        }




                    }

                    if (classifiedAdsList is null)
                    {
                        return NotFound();
                    }
                    Ads = classifiedAdsList;

                    return new JsonResult(classifiedAdsList);
                    //return Redirect("/Ads?catId=" + catId);
                }

                catch (Exception ex)
                {
                    return Page();
                }



            }
        } 
    }

    

