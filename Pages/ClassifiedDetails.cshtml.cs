using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
using NToastNotify;
using Microsoft.EntityFrameworkCore;

namespace Vision.Pages
{
    public class ClassifiedDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private IToastNotification _toastNotification { get; }
        [BindProperty]
        public ApplicationUser user { get; set; }
        public HttpClient httpClient { get; set; }
        [BindProperty]
        public ClassifiedAd ClassifiedAds{ get; set; }
        public int ClassifiedFavCount{ get; set; }
        [BindProperty]
        public long ClassifiedAdId{ get; set; }

        public int ClassifiedCountByUser{ get; set; }
        public string ClassifiedTitle{ get; set; }
        public string Price { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public ClassifiedDetailsModel(CRMDBContext Context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _context = Context;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> OnGet(int ClassifiedId)
        {
            ClassifiedAds = _context.ClassifiedAds.Include(e=>e.ClassifiedAdsCategory).Where(e => e.ClassifiedAdId == ClassifiedId).FirstOrDefault();
            ClassifiedFavCount = _context.FavouriteClassifieds.Where(e=>e.ClassifiedAdId==ClassifiedAds.ClassifiedAdId).Count();
            if (ClassifiedAds != null)
            {
                ClassifiedAdId = ClassifiedAds.ClassifiedAdId;
                ClassifiedAds.Views = ClassifiedAds.Views==null?0: ClassifiedAds.Views + 1;
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
                var TempObjForMap = _context.AdTemplateConfigs.Where(e => e.FieldTypeId == 14 && e.ClassifiedAdsCategoryId == ClassifiedAds.ClassifiedAdsCategoryId ).FirstOrDefault();
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

                user = await _userManager.FindByIdAsync(ClassifiedAds.UseId);
                ClassifiedCountByUser = _context.ClassifiedAds.Where(e => e.UseId == ClassifiedAds.UseId).Count();
                
            }
            return Page();
            
        }
        public async Task<IActionResult> OnPostClassifiedDetails([FromBody] int num)
        {

            var ClassifiedAd = _context.ClassifiedAds.Where(c => c.ClassifiedAdId == num).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new
            {
                ClassifiedAdId = c.ClassifiedAdId,
                ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                IsActive = c.IsActive,
                PublishDate = c.PublishDate,
                UseId = c.UseId,
                Views = c.Views,
                AdContents = c.AdContents.Select(l => new
                {
                    AdContentId = l.AdContentId,
                    AdTemplateConfigId = l.AdTemplateConfigId,
                    FieldTypeId = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                    AdTemplateFieldCaptionAr = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                    AdTemplateFieldCaptionEn = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,

                    AdContentValues = l.AdContentValues.Select(k => new
                    {
                        AdContentValueId = k.AdContentValueId,
                        //ContentValue = k.ContentValue,
                        ContentValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13? _context.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                        
                    }).ToList()


                }).ToList(),

            }).FirstOrDefault();

            return new JsonResult(ClassifiedAd);
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
