using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;
namespace Vision.Areas.CRM.Pages.Configurations.ManageClasifiedChart
{
    public class AdsDetailsModel : PageModel
    {
        private readonly CRMDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public MainInfoAdsDetailsVM mainInfo { get; set; }
        public _LocationVM locationVM { get; set; }
        public List<AdsImage> adsImages { get; set; }
        public ClassifiedAd classifiedAd { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public AdsDetailsModel(CRMDBContext dbContext, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            adsImages = new List<AdsImage>();
            mainInfo = new MainInfoAdsDetailsVM();
            locationVM = new _LocationVM();
            _toastNotification = toastNotification;

        }
        public void OnGet(int classifiedAdsId)
        {
            try
            {
                classifiedAd = _dbContext.ClassifiedAds.Where(e => e.ClassifiedAdId == classifiedAdsId).FirstOrDefault();
                if (classifiedAd != null)
                {
                    var city = _dbContext.Cities.Where(e => e.CityId == classifiedAd.CityId).FirstOrDefault();
                    var area = _dbContext.Areas.Where(e => e.AreaId == classifiedAd.AreaId).FirstOrDefault();

                    var user = _userManager.Users.Where(e => e.Id == classifiedAd.UseId).FirstOrDefault();
                    adsImages = _dbContext.AdsImages.Where(e => e.ClassifiedAdId == classifiedAd.ClassifiedAdId).ToList();
                    mainInfo.AdsTitleEN = classifiedAd.TitleEn;
                    mainInfo.AdsTitleAr = classifiedAd.TitleAr;
                    mainInfo.PublishDate = classifiedAd.PublishDate.Value;
                    mainInfo.FullName = user.FullName;
                    mainInfo.Phone = classifiedAd.PhoneNumber;
                    mainInfo.price = classifiedAd.Price;
                    mainInfo.views = classifiedAd.Views.Value;
                    mainInfo.Active = classifiedAd.IsActive;
                    var locationconcat = city.CityTlEn + " " + area.AreaTlEn;
                    locationVM.location = locationconcat;
                    if (classifiedAd.Location != null)
                    {
                        var location = classifiedAd.Location.Split(',');
                        if (location.Length > 0)
                        {
                            Lat = double.Parse(location[0]);
                            Lng = double.Parse(location[1]);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

            }
        }

        public async Task<IActionResult> OnPostClassifiedDetails([FromBody] int num)
        {

            var ClassifiedAd = _dbContext.ClassifiedAds.Where(c => c.ClassifiedAdId == num).Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Select(c => new
            {
                ClassifiedAdId = c.ClassifiedAdId,
                ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                IsActive = c.IsActive,
                PublishDate = c.PublishDate,
                UseId = c.UseId,
              
                Views = c.Views,
                TitleAr = c.TitleAr,
                TitleEn = c.TitleEn,
                Price = c.Price,
                MainPic = c.MainPic,
                Reel = c.Reel,
                Location = c.Location,
                PhoneNumber = c.PhoneNumber,
               
                Description = c.Description,
                City = _dbContext.Cities.Where(e => e.CityId == c.CityId).Select(k => new
                {
                    k.CityId,
                    k.CityTlAr,
                    k.CityTlEn,

                }).FirstOrDefault(),
                Arae = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).Select(k => new
                {
                    k.AreaId,
                    k.AreaTlAr,
                    k.AreaTlEn,

                }).FirstOrDefault(),


                AdsImages = c.AdsImages.Select(c => new
                {
                    c.AdsImageId,
                    c.Image,
                }).ToList(),

                AdContents = c.AdContents.Select(l => new
                {
                    AdContentId = l.AdContentId,
                    AdTemplateConfigId = l.AdTemplateConfigId,

                    AdContentValues = l.AdContentValues.Select(k => new
                    {
                        AdContentValueId = k.AdContentValueId,
                        //ContentValue = k.ContentValue,
                        ContentValueEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                        ContentValueAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionAr : k.ContentValue,

                        //CheckBoxMultiple = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 6 ?  String.Join(", ", _dbContext.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).Select(c => c.OptionEn).ToArray()): k.ContentValue,

                        FieldTypeId = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                        AdTemplateFieldCaptionAr = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                        AdTemplateFieldCaptionEn = _dbContext.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
                    }).ToList()


                }).ToList(),

            }).FirstOrDefault();

            return new JsonResult(ClassifiedAd);
        }

    }
}
