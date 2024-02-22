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
    public class AdsTestModel : PageModel
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
        //public List<AdTemplateConfig> lstColumns { get; set; }
        public AdsTestModel(IRazorPartialToStringRenderer renderer, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _renderer = renderer;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _context = Context;
            _toastNotification = toastNotification;
            AdsList = new List<ClassifiedAd>();
        }

        public async Task<IActionResult> OnGet(/*int catid, int? selectedValue*/)
        {
            var CountryNames = _context.Cities.ToList().Select(e => e.CityTlEn);
            var OptionList = _context.AdTemplateOptions.ToList();
            var ArabicRanList = OptionList.Select(e => e.OptionAr);
            var EnglishRanList = OptionList.Select(e => e.OptionEn);
            var AreasIds = _context.Areas.ToList().Select(e => e.AreaId);
            var CitesIds = _context.Cities.ToList().Select(e => e.CityId);
            var ListOfImages = new List<string>()
            {
                "Images/ClassifiedAds/f85a73ef-25dd-4604-a8a8-6296111f000c_1690298245993.jpg",
                "Images/ClassifiedAds/26cddd09-dacb-4368-b3a3-5c91591411c1_image_picker_1351327C-335B-443D-B8C4-ABABAB4661D3-43965-000005EEE35FE2DA.jpg",
                "Images/ClassifiedAds/f3390ad1-9b22-4948-beef-e5434b84fae6_IMG_20230706_092234.jpg",
                "Images/ClassifiedAds/1106ffad-c48d-456f-a900-c5df736b7d3a_IMG_20230706_092306.jpg"

            };
            //var ListOfDays = new List<string>()
            //{
            //    "SunDay",
            //    "MonDay",
            //    "Tuesday",
            //    "WedenDay",
            //    "thurthDay",
            //    "SaterDay",
            //};
            //var ListOfTimes = new List<string>()
            //{
            //    "9:58 AM",
            //    "4:58 AM",
            //    "4:58 AM",
            //    "6:58 AM",
            //    "9:59 AM",
            //    "3:59 AM",
            //    "6:59 AM",
            //    "5:59 AM",
            //};
            var random = new Random();
            var adImagesList = new List<AdsImage>();
            //Generate Random Ads
            var ListOfCatagory = new List<int>() { 857, 200 };
            var CatsAds = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == 108).ToList();
            //foreach (var itemEle in CatsAds)
            //{

            for (int i = 0; i < 200; i++)
            {
                foreach (var item in ListOfImages)
                {
                    var adZImages = new AdsImage()
                    {
                        Image = item
                    };
                    adImagesList.Add(adZImages);
                }
                var tags = "";
                var classifiedAds = new ClassifiedAd()
                {
                    TitleAr = ArabicRanList.OrderBy(s => Guid.NewGuid()).First(),
                    TitleEn = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
                    CityId = CitesIds.OrderBy(s => Guid.NewGuid()).First(),
                    AreaId = AreasIds.OrderBy(s => Guid.NewGuid()).First(),
                    Price = random.Next(300, 1300),
                    IsActive = true,
                    Location = "31.271444,30.0106602",
                    PhoneNumber = (random.Next(780201, 98220144)).ToString(),
                    Description = EnglishRanList.OrderBy(s => Guid.NewGuid()).First() + " With Price " + random.Next(300, 1300) + " and In " + CountryNames.OrderBy(s => Guid.NewGuid()).First(),
                    ClassifiedAdsCategoryId = 108,
                    PublishDate = DateTime.Now,
                    Views = 3,
                    MainPic = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
                    UseId = "c89657ba-6692-4c07-8914-2cc3cf136582",
                    Reel = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4",
                    AdsImages = adImagesList,

                };
                _context.ClassifiedAds.Add(classifiedAds);
                _context.SaveChanges();
                var AdTemplateConfig = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == 152).ToList();
                foreach (var item in AdTemplateConfig)
                {
                    if (item.FieldTypeId == 1)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 2)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = (random.Next(2000, 5000)).ToString(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 3 || item.FieldTypeId == 13 || item.FieldTypeId == 6 || item.FieldTypeId == 5)
                    {
                        var Opt = _context.AdTemplateOptions.Where(e => e.AdTemplateConfigId == item.AdTemplateConfigId).Select(e => e.AdTemplateOptionId);
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = Opt.OrderBy(s => Guid.NewGuid()).First().ToString(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        int AdTemplateOption = 0;
                        bool checkParsing = int.TryParse(adcoObj.ContentValue, out AdTemplateOption);
                        if (checkParsing)
                        {
                            var optionValues = _context.AdTemplateOptions.Where(e => e.AdTemplateOptionId == AdTemplateOption).FirstOrDefault();
                            if (tags == "")
                            {
                                tags = optionValues.OptionEn + ";" + optionValues.OptionAr;

                            }
                            else
                            {
                                tags = tags + ";" + optionValues.OptionEn + ";" + optionValues.OptionAr;

                            }
                        }
                        _context.AdContents.AddRange(adContent);
                    }

                    if (item.FieldTypeId == 14)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = "31.271444,30.0106602",
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 7)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 8)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 4)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = DateTime.Now.ToString(),
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 9)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        foreach (var Ele in ListOfImages)
                        {
                            var adcoObj = new AdContentValue()
                            {
                                ContentValue = Ele
                            };
                            adContentValuesList.Add(adcoObj);
                        }

                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }
                    if (item.FieldTypeId == 10 || item.FieldTypeId == 11)
                    {
                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
                        var adcoObj = new AdContentValue()
                        {
                            ContentValue = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4"
,
                        };
                        adContentValuesList.Add(adcoObj);
                        var adContent = new AdContent()
                        {
                            AdTemplateConfigId = item.AdTemplateConfigId,
                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
                            AdContentValues = adContentValuesList
                        };
                        _context.AdContents.AddRange(adContent);
                    }



                }
                classifiedAds.Tags = tags;
                _context.Attach(classifiedAds).State = EntityState.Modified;
                _context.SaveChanges();
            }
        
            ////End Add Random Ads////////////////
























            //var selectedValue1 = Request.Query["controlValue"];
            //ClassifiedAdsCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == catid).ToList();
            //if (ClassifiedAdsCategory.Count == 0)
            //{
            //    var cat = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == catid).FirstOrDefault();
            //    ClassifiedAdsCategory.Add(cat);
            //}
            //Cities = _context.Cities.Where(e => e.CityIsActive == true).ToList();
            //catId = catid;
            //var catList = _context.SearchEntities.Where(e => e.ClassifiedAdsCatagoryId == catid).Select(e => e.SearchCatagoryLevel).ToList();
            //ClassifiedAds = _context.ClassifiedAds.Where(a => a.IsActive == true).Include(e => e.ClassifiedAdsCategory).Where(e => catList.Contains(e.ClassifiedAdsCategoryId.Value)).ToList();
            //if (Ads.Count != 0)
            //{
            //    ClassifiedAds = Ads;

            //}
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
            var Result = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == CatId).OrderBy(e => e.SortOrder).Select(c => new
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
                //var classifiedAdsList = _context.ClassifiedAds.Include(c => c.AdsImages).Include(c => c.AdContents).ThenInclude(c => c.AdContentValues).Where(a => a.ClassifiedAdsCategoryId == catId).Select(c => new
                //{
                //    ClassifiedAdId = c.ClassifiedAdId,
                //    ClassifiedAdsCategoryId = c.ClassifiedAdsCategoryId,
                //    IsActive = c.IsActive,
                //    PublishDate = c.PublishDate,
                //    UseId = c.UseId,
                //    Views = c.Views,
                //    TitleAr = c.TitleAr,
                //    TitleEn = c.TitleEn,
                //    Price = c.Price,
                //    CityId = c.CityId,
                //    AreaId = c.AreaId,
                //    MainPic = c.MainPic,
                //    Description = c.Description,
                //    AdsImages = c.AdsImages.Select(c => new
                //    {
                //        c.AdsImageId,
                //        c.Image,
                //    }).ToList(),
                //    AdContents = c.AdContents.Select(l => new
                //    {
                //        AdContentId = l.AdContentId,
                //        AdTemplateConfigId = l.AdTemplateConfigId,

                //        AdContentValues = l.AdContentValues.Select(k => new
                //        {
                //            AdContentValueId = k.AdContentValueId,
                //            ContentValue = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId == 3 ? _context.AdTemplateOptions.Where(e => e.AdTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,
                //            FieldTypeId = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().FieldTypeId,
                //            AdTemplateFieldCaptionAr = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionAr,
                //            AdTemplateFieldCaptionEn = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == l.AdTemplateConfigId).FirstOrDefault().AdTemplateFieldCaptionEn,
                //        }).ToList()


                //}).ToList(),

                //    }).ToList();
                if (classifiedAdsList != null)
                {
                    //if (classifiedFilterVm.CityId != 0)
                    //{
                    //    classifiedAdsList = classifiedAdsList.Where(e => e.CityId == classifiedFilterVm.CityId).ToList();
                    //}
                    //if (classifiedFilterVm.AreaId != 0)
                    //{
                    //    classifiedAdsList = classifiedAdsList.Where(e => e.AreaId == classifiedFilterVm.AreaId).ToList();
                    //}
                    //if (classifiedFilterVm.FromPrice != 0 && classifiedFilterVm.ToPrice != 0)
                    //{
                    //    classifiedAdsList = classifiedAdsList.Where(e => e.Price > classifiedFilterVm.FromPrice && e.Price < classifiedFilterVm.ToPrice).ToList();
                    //}
                    if (newFilterAds != null)
                    {
                        foreach (var item in newFilterAds)
                        {
                            if (item.AdTemplateConfigId != 0)
                            {
                                var Values = _context.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId && item.value.Contains(e.ContentValue)).Select(e => e.ContentValue).ToList();
                                //var Values = await _context.AdContentValues.Include(e => e.AdContent).Where(e => e.AdContent.AdTemplateConfigId == item.AdTemplateConfigId &&  e.ContentValue == item.Values[0]).Select(e => e.ContentValue).ToListAsync();
                                ClassifiedIds = _context.AdContents.Include(e => e.AdContentValues).Where(e => Values.Contains(e.AdContentValues.FirstOrDefault().ContentValue)).Select(e => e.ClassifiedAdId).ToList();
                                classifiedAdsList = classifiedAdsList.Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();
                                //classifiedAdsList = classifiedAdsList.Single(c => c.category_id == categoryNumber)Where(e => ClassifiedIds.Contains(e.ClassifiedAdId)).ToList();

                            }

                        }
                    }




                }

                if (classifiedAdsList is null)
                {
                    return NotFound();
                }
                //classifiedAdsList = classifiedAdsList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
