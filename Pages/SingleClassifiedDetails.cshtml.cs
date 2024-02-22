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

namespace Vision.Pages
{
#nullable disable
    public class SingleClassifiedDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;


        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IWebHostEnvironment _hostEnvironment;
        public List<AdTemplateConfig> lstColumns { get; set; }
        private readonly ApplicationDbContext _dbContext;
        public HttpClient httpClient { get; set; }

        public string FormTemplate { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
         public ApplicationUser user { get; set; }
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        public SingleClassifiedDetailsModel(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IRazorPartialToStringRenderer renderer, CRMDBContext Context, ApplicationDbContext dbContext)
        {
            _context = Context;
            _dbContext = dbContext;

            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;

        }
        public async Task<IActionResult>OnGet(long classifiedId)
        {
            
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            var classifiedAds = await _context.ClassifiedAds.Where(e => e.ClassifiedAdId == classifiedId).Include(e => e.AdsImages).Include(e => e.AdContents).ThenInclude(e => e.AdContentValues).FirstOrDefaultAsync();

            if (classifiedAds != null)
            {
                lstColumns = _context.AdTemplateConfigs.Where(c => c.ClassifiedAdsCategoryId == classifiedAds.ClassifiedAdsCategoryId).ToList();
            }
            var user = await _userManager.FindByIdAsync(classifiedAds.UseId);

            var catId = classifiedAds.ClassifiedAdsCategoryId;
            classifiedAds.Views = classifiedAds.Views == null ? 0 : classifiedAds.Views + 1;
            _context.SaveChanges();
            var favouriteCount = _context.FavouriteClassifieds.Where(e => e.ClassifiedAdId == classifiedAds.ClassifiedAdId).Count();
            bool isFound = System.IO.File.Exists(_hostEnvironment.WebRootPath + $"\\DisplayAdsTemplates\\{catId}.txt");
            if (isFound == false)
            {
                return Redirect("/PageNF");
            }
            FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + $"\\DisplayAdsTemplates\\{catId}.txt");
            string AdsTiltle = " ";
            string AdsDescription = " ";
            double AdsPrice =0;
            string catTiltle = " ";
            string viewTiltle = " ";
            string CurrencyTiltle = " ";
            string AdsDetailsTiltle = " ";
            string PriceTiltle = " ";
            string DescriptionTiltle = " ";
            string BookmarkTiltle = " ";
            string PhoneTiltle = " ";
            string SalaryTiltle = " ";
            string UserName = "Shenouda Karam";
            string UserPic = "Images/ProfileImages/1f6d55ec-cecb-488b-bef1-e73e041f1d7f_pic_me.jpg";
            string UserPhone = "01289921554";
            if (user != null)
            {
                UserName = user.FullName;
                UserPic = user.ProfilePicture;
                UserPhone = user.PhoneNumber;

            }
            
           



            string views = classifiedAds.Views.Value.ToString();
            //string userName = user.FullName;
            //string userPic = user.ProfilePicture;
            if (BrowserCulture == "en-US")
            {
                catTiltle = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == classifiedAds.ClassifiedAdsCategoryId).FirstOrDefault().ClassifiedAdsCategoryTitleEn;
                viewTiltle = "Viewed";
                CurrencyTiltle = "KD";
                AdsDetailsTiltle = "Ads Details";
                PriceTiltle = "Price";
                DescriptionTiltle = "Description";
                BookmarkTiltle = "Bookmark";
                PhoneTiltle = "Phone";
                SalaryTiltle = "Salary";
                AdsTiltle = classifiedAds.TitleEn;

            }
            else
            {
                catTiltle = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == classifiedAds.ClassifiedAdsCategoryId).FirstOrDefault().ClassifiedAdsCategoryTitleAr;
                viewTiltle = "عدد المشاهدات";
                CurrencyTiltle = "دينار";
                AdsDetailsTiltle = "تفاصيل الإعلان";
                PriceTiltle = "السعر";
                DescriptionTiltle = "الوصف";
                BookmarkTiltle = "عدد الإعجابات";
                PhoneTiltle = "رقم التليفون";
                SalaryTiltle = "الراتب";
                AdsTiltle = classifiedAds.TitleAr;
            }
            AdsPrice = classifiedAds.Price;
            AdsDescription = classifiedAds.Description;

            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0000"), catTiltle);
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0001"), views);
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0002"), viewTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0003"), UserName.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0004"), UserPic.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0005"), CurrencyTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0006"), AdsDetailsTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0007"), PriceTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0008"), DescriptionTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0009"), favouriteCount.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00010"), BookmarkTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00011"), classifiedAds.UseId.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00012"), PhoneTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00013"), UserPhone.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00014"), SalaryTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00020"), AdsTiltle.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00021"), AdsPrice.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00022"), AdsDescription.ToString());
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00023"), await _renderer.RenderPartialToStringAsync("ComShowGalary", classifiedAds));

            //FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "333"), await _renderer.RenderPartialToStringAsync("ComShowUser", user));
            //FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0002"), userPic);

            foreach (var col in lstColumns)
            {
                switch (col.FieldTypeId)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 7:
                        //Get Single Content
                        var VarContent = classifiedAds.AdContents.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault();
                        if (VarContent != null)
                        {
                            var varContentVal = VarContent.AdContentValues.FirstOrDefault();
                            if (varContentVal != null)
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), varContentVal.ContentValue.ToString());
                            }
                            
                        }
                        else
                        {
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), " ");

                        }
                        var LabelValue = " ";
                        var LabelPlusConfig = "0" + col.AdTemplateConfigId.ToString();
                        if (BrowserCulture == "en-US")
                        {
                            LabelValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn;

                        }
                        else
                        {
                            LabelValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr;


                        }
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", LabelPlusConfig), LabelValue);


                        break;
                    case 3: //Lookup
                            //Get Single Content
                        if (classifiedAds.AdContents.Where(c => c.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault() != null)
                        {
                            int AC = Convert.ToInt32(classifiedAds.AdContents.Where(c => c.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdContentValues.FirstOrDefault().ContentValue.ToString());
                            var strLookup = _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == AC).FirstOrDefault().OptionEn;

                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), strLookup);

                        }
                        else
                        {
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), " ");

                        }
                        var optionValue = " ";
                        var optionPlusConfig = "0" + col.AdTemplateConfigId.ToString();
                        if (BrowserCulture == "en-US")
                        {
                            optionValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn;

                        }
                        else
                        {
                            optionValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr;


                        }
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", optionPlusConfig), optionValue);

                        break;
                    case 6: //checkboxlist
                        int dc = Convert.ToInt32(classifiedAds.AdContents.Where(c => c.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault().AdContentValues.FirstOrDefault().ContentValue.ToString());
                        var strcheckboxlist = String.Join(", ", _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == dc).Select(c => c.OptionEn).ToArray());
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), strcheckboxlist);
                        break;
                    //case 9:
                    //    //Photos --> Slider
                    //    FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowGalary", classifiedAds.AdContents.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault()));
                    //    break;
                    case 14:
                        //Map 
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.AdTemplateConfigId), await _renderer.RenderPartialToStringAsync("ComShowMap", classifiedAds.AdContents.Where(e => e.AdTemplateConfigId == col.AdTemplateConfigId).FirstOrDefault()));
                        break;
                    default:
                        break;
                }



            }





            
            return Page();

        }
        public async Task<ApplicationUser> getUserDetails(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            return user;
        }
    }

}
