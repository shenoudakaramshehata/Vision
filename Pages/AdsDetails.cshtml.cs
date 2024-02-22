using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using System.Net.Http;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;

namespace Vision.Pages
{
    public class AdsDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification { get; }
        [BindProperty]
        public ApplicationUser user { get; set; }
        public HttpClient httpClient { get; set; }
        [BindProperty]
        public ClassifiedAd ClassifiedAds { get; set; }
        public ClassifiedBusiness Business { get; set; }
        public List<ClassifiedAd> VendorProducts { get; set; }
        public List<ClassifiedAd> RelatedProducts { get; set; }
        public int ClassifiedFavCount { get; set; }
        [BindProperty]
        public long ClassifiedAdId { get; set; }

        public int ClassifiedCountByUser { get; set; }
        public string ClassifiedTitle { get; set; }
        public string Price { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }


        public AdsDetailsModel(CRMDBContext Context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _context = Context;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }
            public async Task<ActionResult> OnGetAsync()
        {
            ClassifiedAds = _context.ClassifiedAds.Include(e => e.ClassifiedAdsCategory).Where(e => e.ClassifiedAdId == 173).FirstOrDefault();
            RelatedProducts = _context.ClassifiedAds.Where(e => e.ClassifiedAdsCategoryId == ClassifiedAds.ClassifiedAdsCategoryId & e.IsActive).ToList();
            VendorProducts = _context.ClassifiedAds.Where(e => e.UseId == ClassifiedAds.UseId & e.IsActive).ToList();
            user = await _userManager.FindByIdAsync(ClassifiedAds.UseId);
            Business = _context.ClassifiedBusiness.Where(e => e.UseId == ClassifiedAds.UseId).FirstOrDefault();
            ClassifiedFavCount = _context.FavouriteClassifieds.Where(e => e.ClassifiedAdId == ClassifiedAds.ClassifiedAdId).Count();
            if (ClassifiedAds != null)
            {
                ClassifiedAdId = ClassifiedAds.ClassifiedAdId;
                ClassifiedAds.Views = ClassifiedAds.Views == null ? 0 : ClassifiedAds.Views + 1;
                _context.SaveChanges();

                var TempObjForTitle = _context.AdTemplateConfigs.Where(e => e.FieldTypeId == 1 && e.ClassifiedAdsCategoryId == ClassifiedAds.ClassifiedAdsCategoryId && e.AdTemplateFieldCaptionAr.Contains("العنوان") && e.AdTemplateFieldCaptionEn.Contains("Title")).FirstOrDefault();
                if (TempObjForTitle != null)
                {
                    var contentObj = _context.AdContents.Where(e => e.ClassifiedAdId == ClassifiedAds.ClassifiedAdId && e.AdTemplateConfigId == TempObjForTitle.AdTemplateConfigId).FirstOrDefault();
                    if (contentObj != null)
                    {
                        ClassifiedTitle = _context.AdContentValues.Where(e => e.AdContentId == contentObj.AdContentId).FirstOrDefault().ContentValue;
                    }
                }
                var TempObjForPrice = _context.AdTemplateConfigs.Where(e => e.FieldTypeId == 2 && e.ClassifiedAdsCategoryId == ClassifiedAds.ClassifiedAdsCategoryId && e.AdTemplateFieldCaptionAr.Contains("السعر") && e.AdTemplateFieldCaptionEn.Contains("Price")).FirstOrDefault();
                if (TempObjForPrice != null)
                {
                    var contentObj = _context.AdContents.Where(e => e.ClassifiedAdId == ClassifiedAds.ClassifiedAdId && e.AdTemplateConfigId == TempObjForPrice.AdTemplateConfigId).FirstOrDefault();
                    if (contentObj != null)
                    {
                        Price = _context.AdContentValues.Where(e => e.AdContentId == contentObj.AdContentId).FirstOrDefault().ContentValue;
                    }
                }
                var TempObjForMap = _context.AdTemplateConfigs.Where(e => e.FieldTypeId == 14 && e.ClassifiedAdsCategoryId == ClassifiedAds.ClassifiedAdsCategoryId).FirstOrDefault();
                if (TempObjForMap != null)
                {
                    var contentObj = _context.AdContents.Where(e => e.ClassifiedAdId == ClassifiedAds.ClassifiedAdId && e.AdTemplateConfigId == TempObjForMap.AdTemplateConfigId).FirstOrDefault();
                    if (contentObj != null)
                    {
                        var MapValue = _context.AdContentValues.Where(e => e.AdContentId == contentObj.AdContentId).FirstOrDefault().ContentValue;
                        if (MapValue != null)
                        {
                            string[] subs = MapValue.Split(',');
                            Lat = subs[0];
                            Lng = subs[1];
                        }
                    }
                }

                
                ClassifiedCountByUser = _context.ClassifiedAds.Where(e => e.UseId == ClassifiedAds.UseId).Count();

            }
            return Page();
        }
    }
}
