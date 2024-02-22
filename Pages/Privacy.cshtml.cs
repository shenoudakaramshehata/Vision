using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;

namespace Vision.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly CRMDBContext _context;

        public PrivacyModel(CRMDBContext Context, ILogger<PrivacyModel> logger)
        {
            _context = Context;
            _logger = logger;
            _context = Context;
        }
        public void DELETETemplateConfig()
        {
            var MODELS = _context.AdTemplateConfigs.Include(e => e.AdTemplateOptions).Where(e => e.ClassifiedAdsCategoryId == 677).ToList();

            foreach (var item in MODELS)
            {
                var options = _context.AdTemplateOptions.Where(e => e.AdTemplateConfigId == item.AdTemplateConfigId);
                if (options != null)
                {
                    _context.RemoveRange(options);
                    _context.SaveChanges();
                }

            }
            _context.RemoveRange(MODELS);
            _context.SaveChanges();
        }


        //public void years()
        //{
        //    var templateconfig = _context.AdTemplateConfigs.Where(e => e.AdTemplateConfigId == 1738).FirstOrDefault();
        //    List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
        //    for (int i = 1950; i < 2024; i++)
        //    {
        //        AdTemplateOption adTemplate = new AdTemplateOption()
        //        {
        //            AdTemplateConfigId = templateconfig.AdTemplateConfigId,
        //            OptionAr = i.ToString(),
        //            OptionEn = i.ToString()
        //        };
        //        adTemplateOptions.Add(adTemplate);

        //    }
        //    templateconfig.AdTemplateOptions = adTemplateOptions;
        //    _context.SaveChanges();
        //}
        //public void copy()
        //{

        //    var templateconfig = _context.AdTemplateConfigs.Include(e => e.AdTemplateOptions).Where(e => e.ClassifiedAdsCategoryId == 981);
        //    var modelcategry = _context.ClassifiedAdsCategories.Include(e => e.AdTemplateConfigs).ThenInclude(e => e.AdTemplateOptions).Where(e => e.ClassifiedAdsCategoryId == 981).FirstOrDefault();
        //    var parentCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId == modelcategry.ClassifiedAdsCategoryParentId).FirstOrDefault();
        //    var childCategories = _context.ClassifiedAdsCategories.Include(e => e.AdTemplateConfigs).ThenInclude(e => e.AdTemplateOptions).Where(e => e.ClassifiedAdsCategoryParentId == parentCategory.ClassifiedAdsCategoryId).ToList();
        //    foreach (var item in childCategories)
        //    {

        //        if (item.ClassifiedAdsCategoryId != modelcategry.ClassifiedAdsCategoryId)
        //        {
        //            List<AdTemplateConfig> adTemplateConfigs = new List<AdTemplateConfig>();
        //            foreach (var config in templateconfig.ToList())
        //            {
        //                List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();


        //                AdTemplateConfig adTemplateConfigObj = new AdTemplateConfig()
        //                {
        //                    AdTemplateFieldCaptionAr = config.AdTemplateFieldCaptionAr,
        //                    AdTemplateFieldCaptionEn = config.AdTemplateFieldCaptionEn,
        //                    ClassifiedAdsCategoryId = item.ClassifiedAdsCategoryId,
        //                    FieldTypeId = config.FieldTypeId,
        //                    SortOrder = config.SortOrder,
        //                    ValidationMessageAr = config.ValidationMessageAr,
        //                    ValidationMessageEn = config.ValidationMessageEn,
        //                    IsRequired = config.IsRequired
        //                    //AdTemplateOptions = adTemplateOptions
        //                };
        //                _context.AdTemplateConfigs.Add(adTemplateConfigObj);
        //                _context.SaveChanges();
        //                foreach (var ele in config.AdTemplateOptions.ToList())
        //                {
        //                    AdTemplateOption adTemplateOption = new AdTemplateOption()
        //                    {
        //                        AdTemplateConfigId = adTemplateConfigObj.AdTemplateConfigId,
        //                        OptionAr = ele.OptionAr,
        //                        OptionEn = ele.OptionEn,
        //                    };
        //                    _context.AdTemplateOptions.Add(adTemplateOption);
        //                }

        //            }
        //            //_context.AdTemplateConfigs.AddRange(adTemplateConfigs);
        //            _context.SaveChanges();



        //        }
        //    }
        //}

        //private async Task LoadListChildCatagory(int categoryId, List<int> catagoryIds)
        //{
        //    var childCategories = await _context.ClassifiedAdsCategories
        //           .Where(e => e.ClassifiedAdsCategoryParentId == categoryId)
        //           .Select(e => e.ClassifiedAdsCategoryId).ToListAsync();
        //    //catagoryIds.AddRange(childCategories);

        //    foreach (int ChildcategoryId in childCategories)
        //    {
        //        catagoryIds.Add(ChildcategoryId);
        //        await LoadListChildCatagory(ChildcategoryId, catagoryIds);
        //    }
        //}


        public async Task<IActionResult> OnGet()
        {
            //var BDS = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == 7).ToList();
            //var BTC = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == 45).ToList();

            //foreach (var item in BDS)
            //{
            //    if (item.BusinessCategoryId != 239)
            //    {
            //        foreach (var ele in BTC)
            //        {


            //            var options = _context.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == ele.BusinessTemplateConfigId).ToList();

            //            List<BusinessTemplateOption> BDTemplateOptions = new List<BusinessTemplateOption>();
            //            if (options != null)
            //            {
            //                foreach (var opt in options)
            //                {
            //                    BusinessTemplateOption BTemplateOption = new BusinessTemplateOption()
            //                    {
            //                        OptionAr = opt.OptionAr,
            //                        OptionEn = opt.OptionEn,
            //                        ParentId = opt.ParentId,

            //                    };
            //                    BDTemplateOptions.Add(BTemplateOption);
            //                }
            //            }

            //            var BTeConfigObj = new BusinessTemplateConfig()
            //            {
            //                BusinessTemplateFieldCaptionAr = ele.BusinessTemplateFieldCaptionAr,
            //                BusinessTemplateFieldCaptionEn = ele.BusinessTemplateFieldCaptionEn,
            //                HasChild = ele.HasChild,
            //                IsList = ele.IsList,
            //                IsRequired = ele.IsRequired,
            //                FieldTypeId = ele.FieldTypeId,
            //                SortOrder = ele.SortOrder,
            //                ValidationMessageAr = ele.ValidationMessageAr,
            //                ValidationMessageEn = ele.ValidationMessageEn,
            //                Step = ele.Step,
            //                BusinessCategoryId = item.BusinessCategoryId,
            //                ParentId = ele.ParentId,
            //                BusinessTemplateOptions = BDTemplateOptions


            //            };
            //            _context.BusinessTemplateConfigs.Add(BTeConfigObj);
            //            _context.SaveChanges();

            //        }
            //    }


            //}


            //var ListOfCatagory = new List<int>() { 857, 200 };
            //}
            //else
            //{
            //var BDCats = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == 13).ToList();

            //if (BDCats != null)
            //{
            //    foreach (var item in BDCats)
            //    {

            //        List<BusinessTemplateConfig> conBDlIS = new List<BusinessTemplateConfig>();
            //        var configList = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == 5010).ToList();

            //        foreach (var ele in configList)
            //        {


            //            var TempConfig1 = new BusinessTemplateConfig()
            //            {
            //                BusinessCategoryId = item.BusinessCategoryId,
            //                IsRequired = ele.IsRequired,
            //                FieldTypeId = ele.FieldTypeId,
            //                HasChild = ele.HasChild,
            //                IsList = ele.IsList,
            //                ParentId = ele.ParentId,
            //                SortOrder = ele.SortOrder,
            //                Step = ele.Step,
            //                BusinessTemplateFieldCaptionEn = ele.BusinessTemplateFieldCaptionEn,
            //                BusinessTemplateFieldCaptionAr = ele.BusinessTemplateFieldCaptionAr,
            //                ValidationMessageAr = ele.ValidationMessageAr,
            //                ValidationMessageEn = ele.ValidationMessageEn,

            //            };
            //            conBDlIS.Add(TempConfig1);
            //        }
            //        _context.BusinessTemplateConfigs.AddRange(conBDlIS);
            //        _context.SaveChanges();







            //    }
            //}



            //else {
            //var CountryNames = _context.Cities.ToList().Select(e => e.CityTlEn);
            //var OptionList = _context.AdTemplateOptions.ToList();
            //var ArabicRanList = OptionList.Select(e => e.OptionAr);
            //var EnglishRanList = OptionList.Select(e => e.OptionEn);
            //var AreasIds = _context.Areas.ToList().Select(e => e.AreaId);
            //var CitesIds = _context.Cities.ToList().Select(e => e.CityId);
            //var ListOfImages = new List<string>()
            //    {
            //        "Images/ClassifiedAds/f85a73ef-25dd-4604-a8a8-6296111f000c_1690298245993.jpg",
            //        "Images/ClassifiedAds/26cddd09-dacb-4368-b3a3-5c91591411c1_image_picker_1351327C-335B-443D-B8C4-ABABAB4661D3-43965-000005EEE35FE2DA.jpg",
            //        "Images/ClassifiedAds/f3390ad1-9b22-4948-beef-e5434b84fae6_IMG_20230706_092234.jpg",
            //        "Images/ClassifiedAds/1106ffad-c48d-456f-a900-c5df736b7d3a_IMG_20230706_092306.jpg"

            //    };
            //var ListOfDays = new List<string>()
            //    {
            //        "SunDay",
            //        "MonDay",
            //        "Tuesday",
            //        "WedenDay",
            //        "thurthDay",
            //        "SaterDay",
            //    };
            //var ListOfTimes = new List<string>()
            //    {
            //        "9:58 AM",
            //        "4:58 AM",
            //        "4:58 AM",
            //        "6:58 AM",
            //        "9:59 AM",
            //        "3:59 AM",
            //        "6:59 AM",
            //        "5:59 AM",
            //    };
            //var random = new Random();
            //var adImagesList = new List<AdsImage>();



            //Generate Random Ads

            //        var CatsAds = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == 665).ToList();
            //        //foreach (var itemEle in CatsAds)
            //        //{

            //        for (int i = 0; i < 200; i++)
            //            {
            //                foreach (var item in ListOfImages)
            //                {
            //                    var adZImages = new AdsImage()
            //                    {
            //                        Image = item
            //                    };
            //                    adImagesList.Add(adZImages);
            //                }
            //            var tags = "";
            //            var classifiedAds = new ClassifiedAd()
            //                {
            //                    TitleAr = ArabicRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                    TitleEn = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                    CityId = CitesIds.OrderBy(s => Guid.NewGuid()).First(),
            //                    AreaId = AreasIds.OrderBy(s => Guid.NewGuid()).First(),
            //                    Price = random.Next(300, 1300),
            //                    IsActive = true,
            //                    Location = "31.271444,30.0106602",
            //                    PhoneNumber = (random.Next(780201, 98220144)).ToString(),
            //                    Description = EnglishRanList.OrderBy(s => Guid.NewGuid()).First() + " With Price " + random.Next(300, 1300) + " and In " + CountryNames.OrderBy(s => Guid.NewGuid()).First(),
            //                    ClassifiedAdsCategoryId = 152,
            //                    PublishDate = DateTime.Now,
            //                    Views = 3,
            //                    MainPic = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                    UseId = "c89657ba-6692-4c07-8914-2cc3cf136582",
            //                    Reel = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4",
            //                    AdsImages = adImagesList,

            //                };
            //                _context.ClassifiedAds.Add(classifiedAds);
            //                _context.SaveChanges();
            //                var AdTemplateConfig = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == 152).ToList();
            //                foreach (var item in AdTemplateConfig)
            //                {
            //                    if (item.FieldTypeId == 1)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 2)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = (random.Next(2000, 5000)).ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 3 || item.FieldTypeId == 13 || item.FieldTypeId == 6 || item.FieldTypeId == 5)
            //                    {
            //                        var Opt = _context.AdTemplateOptions.Where(e => e.AdTemplateConfigId == item.AdTemplateConfigId).Select(e => e.AdTemplateOptionId);
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = Opt.OrderBy(s => Guid.NewGuid()).First().ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                    int AdTemplateOption = 0;
            //                    bool checkParsing = int.TryParse(adcoObj.ContentValue, out AdTemplateOption);
            //                    if (checkParsing)
            //                    {
            //                        var optionValues = _context.AdTemplateOptions.Where(e => e.AdTemplateOptionId == AdTemplateOption).FirstOrDefault();
            //                        if (tags == "")
            //                        {
            //                            tags = optionValues.OptionEn + ";" + optionValues.OptionAr;

            //                        }
            //                        else
            //                        {
            //                            tags = tags + ";" + optionValues.OptionEn + ";" + optionValues.OptionAr;

            //                        }
            //                    }
            //                    _context.AdContents.AddRange(adContent);
            //                    }

            //                    if (item.FieldTypeId == 14)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = "31.271444,30.0106602",
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 7)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 8)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 4)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = DateTime.Now.ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 9)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        foreach (var Ele in ListOfImages)
            //                        {
            //                            var adcoObj = new AdContentValue()
            //                            {
            //                                ContentValue = Ele
            //                            };
            //                            adContentValuesList.Add(adcoObj);
            //                        }

            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 10 || item.FieldTypeId == 11)
            //                    {
            //                        List<AdContentValue> adContentValuesList = new List<AdContentValue>();
            //                        var adcoObj = new AdContentValue()
            //                        {
            //                            ContentValue = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4"
            //,
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new AdContent()
            //                        {
            //                            AdTemplateConfigId = item.AdTemplateConfigId,
            //                            ClassifiedAdId = classifiedAds.ClassifiedAdId,
            //                            AdContentValues = adContentValuesList
            //                        };
            //                        _context.AdContents.AddRange(adContent);
            //                    }



            //                }
            //            classifiedAds.Tags = tags;
            //            _context.Attach(classifiedAds).State = EntityState.Modified;
            //            _context.SaveChanges();
            //            }
            // }
            //////End Add Random Ads////////////////
            // Add Random BD /////////////
            //var BDCatagories = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == 7).ToList();
            //foreach (var CatEle in BDCatagories)
            //{

            //    for (int i = 0; i < 50; i++)
            //    {
            //        List<BusinessWorkingHours> WHs = new List<BusinessWorkingHours>();
            //        for (int k = 0; k < 2; k++)
            //        {
            //            BusinessWorkingHours bwh = new BusinessWorkingHours()
            //            {
            //                StartTime1 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                StartTime2 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                EndTime1 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                EndTime2 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                Isclosed = false,
            //                Day = ListOfDays.OrderBy(s => Guid.NewGuid()).First(),
            //            };
            //            WHs.Add(bwh);
            //        }
            //        List<BDImage> bDImages = new List<BDImage>();

            //        foreach (var elem in ListOfImages)
            //        {
            //            var bdImageObj = new BDImage()
            //            {
            //                Image = elem
            //            };
            //            bDImages.Add(bdImageObj);
            //        }
            //        var BD = new ClassifiedBusiness()
            //        {
            //            Title = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //            CityId = CitesIds.OrderBy(s => Guid.NewGuid()).First(),
            //            AreaId = AreasIds.OrderBy(s => Guid.NewGuid()).First(),
            //            Deliverycost = random.Next(3, 100),
            //            IsActive = true,
            //            Location = "31.271444,30.0106602",
            //            Address = CountryNames.OrderBy(s => Guid.NewGuid()).First(),
            //            phone = (random.Next(780201, 98220144)).ToString(),
            //            Description = EnglishRanList.OrderBy(s => Guid.NewGuid()).First() + " With Address " + CountryNames.OrderBy(s => Guid.NewGuid()).First(),
            //            BusinessCategoryId = CatEle.BusinessCategoryId,
            //            PublishDate = DateTime.Now,
            //            Views = 3,
            //            Mainpic = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //            Logo = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //            Rating = random.Next(1, 5),
            //            Email = "shenouda" + random.Next(i, 100) + "@gmail.com",
            //            businessWorkingHours = WHs,
            //            UseId = "f8bb360d-121d-4793-9b54-8a5e8fcd1fa4",
            //            Reel = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4",
            //            BDImages = bDImages
            //        };

            //        _context.ClassifiedBusiness.Add(BD);
            //        _context.SaveChanges();
            //        var plan = _context.BussinessPlans.FirstOrDefault();
            //        double totalCost = plan.Price.Value;
            //        var subscription = new BusiniessSubscription();
            //        subscription.BussinessPlanId = plan.BussinessPlanId;
            //        subscription.ClassifiedBusinessId = BD.ClassifiedBusinessId;
            //        subscription.Price = totalCost;
            //        subscription.StartDate = DateTime.Now;
            //        subscription.PaymentMethodId = 1;
            //        subscription.IsActive = true;
            //        subscription.EndDate = DateTime.Now.AddMonths(plan.DurationInMonth.Value);
            //        _context.BusiniessSubscriptions.Add(subscription);
            //        var AdTemplateConfig = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == CatEle.BusinessCategoryId).ToList();
            //        foreach (var item in AdTemplateConfig)
            //        {
            //            if (item.FieldTypeId == 1)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 2)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = (random.Next(2000, 5000)).ToString(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);

            //            }
            //            if (item.FieldTypeId == 3 || item.FieldTypeId == 13 || item.FieldTypeId == 6 || item.FieldTypeId == 5)
            //            {
            //                var Opt = _context.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).Select(e => e.BusinessTemplateOptionId);
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = Opt.OrderBy(s => Guid.NewGuid()).First().ToString(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }

            //            if (item.FieldTypeId == 14)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = "31.271444,30.0106602",
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 7)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 8)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 4)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = DateTime.Now.ToString(),
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 9)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                foreach (var Ele in ListOfImages)
            //                {
            //                    var adcoObj = new BusinessContentValue()
            //                    {
            //                        ContentValue = Ele
            //                    };
            //                    adContentValuesList.Add(adcoObj);
            //                }

            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }
            //            if (item.FieldTypeId == 10 || item.FieldTypeId == 11)
            //            {
            //                List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                var adcoObj = new BusinessContentValue()
            //                {
            //                    ContentValue = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4"
            //        ,
            //                };
            //                adContentValuesList.Add(adcoObj);
            //                var adContent = new BusinessContent()
            //                {
            //                    BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                    ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                    BusinessContentValues = adContentValuesList
            //                };
            //                _context.BusinessContents.AddRange(adContent);
            //            }



            //        }
            //        _context.SaveChanges();
            //    }
            //}
            //    ///End Random BD////////////
            //        var bC = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == 1).ToList();
            //        foreach (var Cat in bC)
            //        {
            //            for (int i = 0; i < 50; i++)
            //            {
            //                List<BusinessWorkingHours> WHs = new List<BusinessWorkingHours>();
            //                for (int k = 0; k < 2; k++)
            //                {
            //                    BusinessWorkingHours bwh = new BusinessWorkingHours()
            //                    {
            //                        StartTime1 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                        StartTime2 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                        EndTime1 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                        EndTime2 = ListOfTimes.OrderBy(s => Guid.NewGuid()).First(),
            //                        Isclosed = false,
            //                        Day = ListOfDays.OrderBy(s => Guid.NewGuid()).First(),
            //                    };
            //                    WHs.Add(bwh);
            //                }

            //                var BD = new ClassifiedBusiness()
            //                {
            //                    Title = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                    CityId = CitesIds.OrderBy(s => Guid.NewGuid()).First(),
            //                    AreaId = AreasIds.OrderBy(s => Guid.NewGuid()).First(),
            //                    Deliverycost = random.Next(3, 100),
            //                    IsActive = true,
            //                    Location = "31.271444,30.0106602",
            //                    Address = CountryNames.OrderBy(s => Guid.NewGuid()).First(),
            //                    phone = (random.Next(780201, 98220144)).ToString(),
            //                    Description = EnglishRanList.OrderBy(s => Guid.NewGuid()).First() + " With Address " + CountryNames.OrderBy(s => Guid.NewGuid()).First(),
            //                    BusinessCategoryId = Cat.BusinessCategoryId,
            //                    PublishDate = DateTime.Now,
            //                    Views = 3,
            //                    Mainpic = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                    Logo = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                    Rating = random.Next(1, 5),
            //                    Email = "shenouda" + i + "@gmail.com",
            //                    businessWorkingHours = WHs,
            //                    UseId = "c89657ba-6692-4c07-8914-2cc3cf136582",
            //                    Reel = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4",
            //                };

            //                _context.ClassifiedBusiness.Add(BD);
            //                _context.SaveChanges();
            //                var AdTemplateConfig = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == Cat.BusinessCategoryId).ToList();
            //                foreach (var item in AdTemplateConfig)
            //                {
            //                    if (item.FieldTypeId == 1)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 2)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = (random.Next(2000, 5000)).ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);

            //                    }
            //                    if (item.FieldTypeId == 3 || item.FieldTypeId == 13 || item.FieldTypeId == 6)
            //                    {
            //                        var Opt = _context.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == item.BusinessTemplateConfigId).Select(e => e.BusinessTemplateOptionId);
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = Opt.OrderBy(s => Guid.NewGuid()).First().ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 5)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = "1",
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }

            //                    if (item.FieldTypeId == 14)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = "31.271444,30.0106602",
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 7)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = EnglishRanList.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 8)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = ListOfImages.OrderBy(s => Guid.NewGuid()).First(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 4)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = DateTime.Now.ToString(),
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 9)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        foreach (var Ele in ListOfImages)
            //                        {
            //                            var adcoObj = new BusinessContentValue()
            //                            {
            //                                ContentValue = Ele
            //                            };
            //                            adContentValuesList.Add(adcoObj);
            //                        }

            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }
            //                    if (item.FieldTypeId == 10 || item.FieldTypeId == 11)
            //                    {
            //                        List<BusinessContentValue> adContentValuesList = new List<BusinessContentValue>();
            //                        var adcoObj = new BusinessContentValue()
            //                        {
            //                            ContentValue = "Images/ClassifiedAds/10c52a84-86b0-46f4-8b17-c9ba6edf306f_Record_2023-07-07-13-57-41.mp4"
            //,
            //                        };
            //                        adContentValuesList.Add(adcoObj);
            //                        var adContent = new BusinessContent()
            //                        {
            //                            BusinessTemplateConfigId = item.BusinessTemplateConfigId,
            //                            ClassifiedBusinessId = BD.ClassifiedBusinessId,
            //                            BusinessContentValues = adContentValuesList
            //                        };
            //                        _context.BusinessContents.AddRange(adContent);
            //                    }



            //                }
            //                _context.SaveChanges();
            //            }
            //        }


            ///delete////////
            // var Brands = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == 857).ToList();
            //foreach (var item in Brands)
            //{
            //    var Models = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == item.ClassifiedAdsCategoryId).ToList();
            //    _context.ClassifiedAdsCategories.RemoveRange(Models);
            //    //foreach (var ele in Models)
            //{
            //    var AdTemplateList = _context.AdTemplateConfigs.Where(e => e.ClassifiedAdsCategoryId == ele.ClassifiedAdsCategoryId).ToList();
            //    if (AdTemplateList != null)
            //    {
            //        foreach (var config in AdTemplateList)
            //        {
            //            var OptionList = _context.AdTemplateOptions.Where(e => e.AdTemplateConfigId == config.AdTemplateConfigId).ToList();
            //            if (OptionList != null)
            //            {
            //                _context.AdTemplateOptions.RemoveRange(OptionList);
            //            }

            //        }
            //        _context.AdTemplateConfigs.RemoveRange(AdTemplateList);
            //        _context.SaveChanges();
            //    }

            //var classAdsListLg = _context.ClassifiedAds.Where(e => e.ClassifiedAdsCategoryId == ele.ClassifiedAdsCategoryId).ToList();
            //var classAdsList = _context.ClassifiedAds.Where(e => e.ClassifiedAdsCategoryId == ele.ClassifiedAdsCategoryId).Select(e => e.ClassifiedAdId).ToList();
            //var AdsContentList = _context.AdContents.Where(e =>classAdsList.Contains(e.ClassifiedAdId)).ToList();
            //var AdsNewContentList = AdsContentList.Select(e => e.AdContentId).ToList();
            //if (AdsContentList != null)
            //{
            //    var ContentValues = _context.AdContentValues.Where(e => AdsNewContentList.Contains(e.AdContentId)).ToList();
            //    _context.AdContentValues.RemoveRange(ContentValues);
            //}
            //var ImageList = _context.AdsImages.Where(e =>classAdsList.Contains(e.ClassifiedAdId)).ToList();
            //if (ImageList != null)
            //{
            //    _context.AdsImages.RemoveRange(ImageList);
            //}
            //_context.ClassifiedAds.RemoveRange(classAdsListLg);
            //}

            //}
            // _context.ClassifiedAdsCategories.RemoveRange(Brands);
            //       _context.SaveChanges();

            // var classifiedCat = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == 857).ToList();
            //foreach (var item in classifiedCat)
            //{
            //    AdTemplateOption adTemplateOption = new AdTemplateOption()
            //    {
            //        OptionAr = item.ClassifiedAdsCategoryTitleAr,
            //        OptionEn = item.ClassifiedAdsCategoryTitleEn,
            //        AdTemplateConfigId = 2226,
            //        ParentId = 0,
            //        Pic = item.ClassifiedAdsCategoryPic,
            //        SortOrder = 1
            //    };
            //    _context.AdTemplateOptions.Add(adTemplateOption);
            //    _context.SaveChanges();
            //    var newclassifiedCat = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryParentId == item.ClassifiedAdsCategoryId).ToList();
            //    List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
            //    foreach (var ele in newclassifiedCat)
            //    {
            //        AdTemplateOption newadTemplateOption = new AdTemplateOption()
            //        {
            //            OptionAr = ele.ClassifiedAdsCategoryTitleAr,
            //            OptionEn = ele.ClassifiedAdsCategoryTitleEn,
            //            AdTemplateConfigId = 2227,
            //            ParentId = adTemplateOption.AdTemplateOptionId,
            //            Pic = ele.ClassifiedAdsCategoryPic,
            //            SortOrder = 1
            //        };
            //        adTemplateOptions.Add(newadTemplateOption);


            //    }
            //    _context.AdTemplateOptions.AddRange(adTemplateOptions);
            //    _context.SaveChanges();
            //}



            //int catid = 1;
            //List<int> catsAdsIds = new List<int>() { catid };
            //await LoadListChildCatagory(catid, catsAdsIds);
            //List<SearchEntity> searchEntitiesList = new List<SearchEntity>();
            //foreach (var item in catsAdsIds)
            //{
            //    var searchEn = new SearchEntity()
            //    {
            //        ClassifiedAdsCatagoryId = catid,
            //        SearchCatagoryLevel = item
            //    };
            //    searchEntitiesList.Add(searchEn);

            //}
            //    _context.SearchEntities.AddRange(searchEntitiesList);
            //    _context.SaveChanges();
            //var classifiedCat = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId==93).Select(e => e.ClassifiedAdsCategoryId).ToList();
            //foreach (var ele in classifiedCat)
            //{
            //    //int catid = 1;
            //    List<int> catsAdsIds = new List<int>() { ele };
            //    await LoadListChildCatagory(ele, catsAdsIds);
            //    List<SearchEntity> searchEntitiesList = new List<SearchEntity>();
            //    foreach (var item in catsAdsIds)
            //    {
            //        var searchEn = new SearchEntity()
            //        {
            //            ClassifiedAdsCatagoryId = ele,
            //            SearchCatagoryLevel = item
            //        };
            //        searchEntitiesList.Add(searchEn);

            //    }
            //    _context.SearchEntities.AddRange(searchEntitiesList);
            //    _context.SaveChanges();
            //}
            //var adTemplateConfigsList = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.ClassifiedAdsCategoryId == 857).ToList();
            //List<AdTemplateConfig> adTemplateConfigs = new List<AdTemplateConfig>();
            //foreach (var item in adTemplateConfigsList)
            //{
            //	List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
            //	foreach (var ele in item.AdTemplateOptions)
            //	{
            //		AdTemplateOption adTemplateOption = new AdTemplateOption()
            //		{
            //			OptionAr = ele.OptionAr,
            //			OptionEn = ele.OptionEn,
            //			OptionEn = ele.OptionEn,
            //		};
            //		adTemplateOptions.Add(adTemplateOption);
            //	}

            //	AdTemplateConfig adTemplateConfigObj = new AdTemplateConfig()
            //	{
            //		AdTemplateFieldCaptionAr = item.AdTemplateFieldCaptionAr,
            //		AdTemplateFieldCaptionEn = item.AdTemplateFieldCaptionEn,
            //		ClassifiedAdsCategoryId = 683,
            //		FieldTypeId = item.FieldTypeId,
            //		SortOrder = item.SortOrder,
            //		ValidationMessageAr = item.ValidationMessageAr,
            //		ValidationMessageEn = item.ValidationMessageEn,
            //		IsRequired = item.IsRequired,
            //		AdTemplateOptions = adTemplateOptions
            //	};
            //	adTemplateConfigs.Add(adTemplateConfigObj);
            //}

            return Page();

                //_context.AdTemplateConfigs.AddRange(adTemplateConfigs);
                //_context.SaveChanges();






                //years();
                //copy();

                //    List<AdTemplateConfig> SourceModel = _context.AdTemplateConfigs.Where(c => c.ClassifiedAdsCategoryId == 980).Include(i => i.AdTemplateOptions).ToList();

                //    ClassifiedAdsCategory TargetModel = _context.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == 981).FirstOrDefault();


                //    foreach (var adTemplateConfig in SourceModel)
                //    {
                //        TargetModel.AdTemplateConfigs.Add(adTemplateConfig);
                //    }

                //_context.SaveChanges();



                //var NotRootCats = _context.SubCategories.Where(e=>e.CategoryId==16).ToList();
                //foreach (var item in NotRootCats)
                //{
                //    var cat = new BusinessCategory()
                //    {
                //        BusinessCategoryTitleAr = item.SubCategoryTitleEn,
                //        BusinessCategoryTitleEn = item.SubCategoryTitleEn,
                //        BusinessCategoryCategoryPic = "p1.jpg",
                //        BusinessCategoryIsActive = true,
                //        BusinessCategorySortOrder = 1,
                //        BusinessCategoryParentId = 17,
                //    };
                //    _context.BusinessCategories.Add(cat);
                //    _context.SaveChanges();

                //}
                //var NotRootCats = _context.BusinessCategories.Where(e=>e.BusinessCategoryId>2).ToList();
                //var AdTemplateConfigs = _context.BusinessTemplateConfigs.Where(e => e.BusinessCategoryId == 1).ToList();
                //foreach (var item in NotRootCats)
                //{
                //    foreach (var elem in AdTemplateConfigs)
                //    {
                //        var tem = new BusinessTemplateConfig()
                //        {
                //            BusinessCategoryId = item.BusinessCategoryId,
                //            BusinessTemplateFieldCaptionAr = elem.BusinessTemplateFieldCaptionAr,
                //            BusinessTemplateFieldCaptionEn = elem.BusinessTemplateFieldCaptionEn,
                //            FieldTypeId = elem.FieldTypeId,
                //            IsRequired = elem.IsRequired,
                //            SortOrder = elem.SortOrder,
                //            ValidationMessageAr = elem.ValidationMessageAr,
                //            ValidationMessageEn = elem.ValidationMessageEn

                //        };
                //        _context.BusinessTemplateConfigs.Add(tem);
                //        _context.SaveChanges();
                //    }

                //}



                //cat.AdTemplateConfigs = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.ClassifiedAdsCategoryId == cat.ClassifiedAdsCategoryParentId).ToList();
                //cat.AdTemplateConfigs = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.ClassifiedAdsCategoryId == 1).ToList();

                //var NotRootCats = _context.BusinessCategories.Where(c => c.BusinessCategoryParentId != null).ToList();
                //var NotRootCats = _context.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == 667).ToList();

                //foreach (var cat in NotRootCats)
                //{
                //    var adTemplateConfigsList = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.AdTemplateConfigId == cat.cl).ToList();
                //    List<AdTemplateConfig> adTemplateConfigs = new List<AdTemplateConfig>();
                //    foreach (var item in adTemplateConfigsList)
                //    {
                //        List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
                //        foreach (var ele in item.AdTemplateOptions)
                //        {
                //            AdTemplateOption adTemplateOption = new AdTemplateOption()
                //            {
                //                AdTemplateConfigId = ele.AdTemplateConfigId,
                //                OptionAr = ele.OptionAr,
                //                OptionEn = ele.OptionEn,
                //            };
                //            adTemplateOptions.Add(adTemplateOption);
                //        }

                //        //BusinessTemplateConfig adTemplateConfigObj = new BusinessTemplateConfig()
                //        //{
                //        //    BusinessTemplateFieldCaptionAr = item.BusinessTemplateFieldCaptionAr,
                //        //    BusinessTemplateFieldCaptionEn = item.BusinessTemplateFieldCaptionEn,
                //        //    BusinessCategoryId = cat.BusinessCategoryId,
                //        //    FieldTypeId = item.FieldTypeId,
                //        //    SortOrder = item.SortOrder,
                //        //    ValidationMessageAr = item.ValidationMessageAr,
                //        //    ValidationMessageEn = item.ValidationMessageEn,
                //        //    IsRequired = item.IsRequired,
                //        //    BusinessTemplateOptions = adTemplateOptions
                //        //};
                //        //adTemplateConfigs.Add(adTemplateConfigObj);
                //    }

                //_context.BusinessTemplateConfigs.AddRange(adTemplateConfigs);
                //_context.SaveChanges();
                //cat.AdTemplateConfigs = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.ClassifiedAdsCategoryId == 2).ToList();
                //cat.AdTemplateConfigs = adTemplateConfigsList;
                //cat.AdTemplateConfigs.Count();
                //adTemplateConfigsList.Count();
                //}
                //}
                // var NotRootCats = _context.ClassifiedAdsCategories.Where(c => c.ClassifiedAdsCategoryId == 667).ToList();


                //var adTemplateConfigsList = _context.AdTemplateConfigs.Include(c => c.AdTemplateOptions).Where(c => c.ClassifiedAdsCategoryId == 685).ToList();
                //List<AdTemplateConfig> adTemplateConfigs = new List<AdTemplateConfig>();
                //foreach (var item in adTemplateConfigsList)
                //{
                //    List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
                //    foreach (var ele in item.AdTemplateOptions)
                //    {
                //        AdTemplateOption adTemplateOption = new AdTemplateOption()
                //        {
                //            OptionAr = ele.OptionAr,
                //            OptionEn = ele.OptionEn,
                //        };
                //        adTemplateOptions.Add(adTemplateOption);
                //    }

                //    AdTemplateConfig adTemplateConfigObj = new AdTemplateConfig()
                //    {
                //        AdTemplateFieldCaptionAr = item.AdTemplateFieldCaptionAr,
                //        AdTemplateFieldCaptionEn = item.AdTemplateFieldCaptionEn,
                //        ClassifiedAdsCategoryId = 686,
                //        FieldTypeId = item.FieldTypeId,
                //        SortOrder = item.SortOrder,
                //        ValidationMessageAr = item.ValidationMessageAr,
                //        ValidationMessageEn = item.ValidationMessageEn,
                //        IsRequired = item.IsRequired,
                //        AdTemplateOptions = adTemplateOptions
                //    };
                //    adTemplateConfigs.Add(adTemplateConfigObj);
                //}
                //_context.AdTemplateConfigs.AddRange(adTemplateConfigs);
                //_context.SaveChanges();
                //var Options = _context.AdTemplateOptions.Where(c => c.AdTemplateConfigId == 2180).ToList();
                //_context.AdTemplateOptions.RemoveRange(Options);
                //_context.SaveChanges();
                //var Options = _context.AdTemplateOptions.Where(c => c.AdTemplateConfigId == 2158).ToList();
                //List<AdTemplateOption> adTemplateOptions = new List<AdTemplateOption>();
                //foreach (var ele in Options)
                //{
                //    AdTemplateOption adTemplateOption = new AdTemplateOption()
                //    {
                //        OptionAr = ele.OptionEn,
                //        OptionEn = ele.OptionAr,
                //        AdTemplateConfigId = 2198
                //    };
                //    adTemplateOptions.Add(adTemplateOption);
                //}
                //_context.AdTemplateOptions.AddRange(adTemplateOptions);
                //_context.SaveChanges();





            }
        }
    }
