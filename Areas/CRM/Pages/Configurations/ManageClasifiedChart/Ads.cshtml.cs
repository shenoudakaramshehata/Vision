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
    public class AdsModel : PageModel
    {
        private readonly CRMDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;

        public List<AdsGrid> classifiedAds { get; set; }
        public AdsModel(CRMDBContext dbContext, UserManager<ApplicationUser> userManager, IToastNotification toastNotification)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            classifiedAds = new List<AdsGrid>();
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
               
                // Retrieve classified ads with related data
                classifiedAds = await _dbContext.ClassifiedAds
                    .Where(a => a.IsActive)
                    .Include(c => c.ClassifiedAdsCategory)
                    
                    .Select(c => new AdsGrid
                    {
                        ClassifiedAdId = c.ClassifiedAdId,
                        ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId.Value,
                        Active = c.IsActive,
                        ClassifiedAdsCategoryTitleEn = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
                        ClassifiedAdsCategoryTitleAr = c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
                        PublishDate = c.PublishDate.Value,
                        Views = c.Views,
                        TitleAr = c.TitleAr,
                        TitleEn = c.TitleEn,
                        Price = c.Price,
                        MainPic = c.MainPic,
                        Description = c.Description,
                        City = _dbContext.Cities.Where(e=>e.CityId==c.CityId).FirstOrDefault().CityTlEn,
                        Area = _dbContext.Areas.Where(e => e.AreaId == c.AreaId).FirstOrDefault().AreaTlEn,
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
                
            }

            return Page();
        }
        //public async Task<IActionResult> OnGet()
        //{
        //    try
        //    {


        //        //var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == CategoryID).Select(e => e.SearchCatagoryLevel).ToList();
        //        classifiedAds = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(c => c.ClassifiedAdsCategory)
        //    .Select( c => new AdsGrid
        //        {
        //            ClassifiedAdId = c.ClassifiedAdId,
        //            ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId.Value,
        //            Active = c.IsActive,
        //            ClassifiedAdsCategoryTitleEn=c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleEn,
        //            ClassifiedAdsCategoryTitleAr= c.ClassifiedAdsCategory.ClassifiedAdsCategoryTitleAr,
        //            PublishDate = c.PublishDate.Value,

        //            Views = c.Views,
        //            TitleAr = c.TitleAr,
        //            TitleEn = c.TitleEn,
        //            Price = c.Price,
        //            MainPic = c.MainPic,
        //            Description = c.Description,
        //          User = await GetUserNameAsync(c.UseId),
        //        City =  _dbContext.Cities.Where(e=>e.CityId==c.CityId).FirstOrDefault().CityTlEn,
        //            Area =  _dbContext.Areas.Where(e => e.AreaId == c.AreaId).FirstOrDefault().AreaTlEn,


        //        }).ToListAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went error");

        //    }
        //    return Page();
        //}
        private async Task<string> GetUserFullNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.FullName;
        }
    }
        //public async Task<IActionResult> GetAdsInCategory(int CategoryID, int page = 1, int pageSize = 10)
        //{
        //	try
        //	{

        //		var catList = _dbContext.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == CategoryID).Select(e => e.SearchCatagoryLevel).ToList();
        //		var classified = await _dbContext.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => e.ClassifiedAdsCategoryId == CategoryID || catList.Contains(e.ClassifiedAdsCategoryId.Value)).Select(c => new
        //		{
        //			ClassifiedAdId = c.ClassifiedAdId,
        //			ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
        //			IsActive = c.IsActive,
        //			PublishDate = c.PublishDate,
        //			UseId = c.UseId,
        //			Views = c.Views,
        //			TitleAr = c.TitleAr,
        //			TitleEn = c.TitleEn,
        //			Price = c.Price,
        //			MainPic = c.MainPic,
        //			Description = c.Description,
        //			user = _userManager.FindByIdAsync(c.UseId),
        //			City = c.CityId,
        //			Area = c.AreaId,


        //		}).ToListAsync();
        //		if (classified != null)
        //		{
        //			classified = classified.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //			return Ok(new { Status = true, ClassifiedAd = classified, Message = "Process completed successfully" });

        //		}

        //		return Ok(new { Status = true, ClassifiedAd = classified, Message = "Process completed successfully" });
        //	}
        //	catch (Exception ex)
        //	{
        //		return Ok(new { Status = false, Message = ex.Message });
        //	}
        //}

    }
    
