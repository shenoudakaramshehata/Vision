using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Vision.ViewModels;
using NToastNotify;

namespace Vision.Pages
{
    #nullable disable
    public class SingleBussinessDetailsModel : PageModel
    {
        public List<BusinessWorkingHours> businessWorkingHours = new List<BusinessWorkingHours>();
        private readonly CRMDBContext _context;


        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IWebHostEnvironment _hostEnvironment;
        public List<BusinessTemplateConfig> lstColumns { get; set; }
        private readonly ApplicationDbContext _dbContext;
        public readonly IToastNotification toastNotification;
        public HttpClient httpClient { get; set; }
        [BindProperty]
        public BusinessUserInfo BusinessUserInfo { get; set; }
        public List<BusinessWorkingHours> businessHours { get; set; }
        public string FormTemplate { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        [BindProperty]
        public Review Review { get; set; }
        public  long ClassifiedBusinessId { get; set; }
        public int? bussinessId { get; set; }
        public FavouriteBusiness favourite { get; set; }
        public ClassifiedBusiness classifiedBuiness { get; set; }
        public int countFav { get; set; }
        public List<Review> reviews { get; set; }
        public ApplicationUser user { get; set; }
        public List<Product> productCategories { get; set; }
        public List<ClassifiedBusiness> similarBusinesses { get; set; }
        public SingleBussinessDetailsModel(IToastNotification _toastNotification,UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IRazorPartialToStringRenderer renderer, CRMDBContext Context, ApplicationDbContext dbContext)
        {
            _context = Context;
            _dbContext = dbContext;
            toastNotification = _toastNotification;
            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            Review = new Review();

        }
        public async Task<IActionResult> OnGet(long BDId)
        {
            locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            BrowserCulture = locale.RequestCulture.UICulture.ToString();
            classifiedBuiness = await _context.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == BDId).Include(e => e.BusinessContents).ThenInclude(e => e.BusinessContentValues).FirstOrDefaultAsync();
            businessHours = _context.BusinessWorkingHours.Where(e => e.ClassifiedBusinessId == classifiedBuiness.ClassifiedBusinessId).ToList();
            ClassifiedBusinessId = classifiedBuiness.ClassifiedBusinessId;
            productCategories = _context.Products.Include(e=>e.ProductCategory).Where(e =>e.ProductCategory.ClassifiedBusinessId== ClassifiedBusinessId && e.IsActive == true).ToList();
            similarBusinesses = _context.ClassifiedBusiness.Where(e => e.BusinessCategoryId == classifiedBuiness.BusinessCategoryId && e.IsActive == true).Include(e => e.BusinessCategory).OrderByDescending(e => e.PublishDate).Take(5).ToList();
            if (classifiedBuiness != null)
            {
                lstColumns = _context.BusinessTemplateConfigs.Where(c => c.BusinessCategoryId == classifiedBuiness.BusinessCategoryId).ToList();
            }
            user = await _userManager.FindByIdAsync(classifiedBuiness.UseId);
            favourite = _context.FavouriteBusiness.Where(a => a.UserId == user.Id && a.ClassifiedBusinessId == classifiedBuiness.ClassifiedBusinessId).FirstOrDefault();
            countFav = _context.FavouriteBusiness.Where(a => a.ClassifiedBusinessId == ClassifiedBusinessId).Count();
            bussinessId = classifiedBuiness.BusinessCategoryId;
            classifiedBuiness.Views = classifiedBuiness.Views == null ? 0 : classifiedBuiness.Views + 1;
            _context.SaveChanges();


            var favouriteCount = _context.FavouriteBusiness.Where(e => e.ClassifiedBusinessId == classifiedBuiness.ClassifiedBusinessId).Count();
            bool isFound = System.IO.File.Exists(_hostEnvironment.WebRootPath + $"\\BusinessDetails\\{classifiedBuiness.BusinessCategoryId}.txt");
            if (isFound == false)
            {
                return Redirect("/PageNF");
            }
            FormTemplate = System.IO.File.ReadAllText(_hostEnvironment.WebRootPath + $"\\BusinessDetails\\{classifiedBuiness.BusinessCategoryId}.txt");

            if (user != null)
            {
                BusinessUserInfo = new BusinessUserInfo()
                {
                    FullName = user.FullName,
                    Pic = user.ProfilePicture,
                    UserId = user.Id
                };
            }
            




            string views = classifiedBuiness.Views.Value.ToString();
            reviews = _context.Reviews.ToList();
               
                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0055"), await _renderer.RenderPartialToStringAsync("BDDisplayUserInfo", BusinessUserInfo));
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "0054"), await _renderer.RenderPartialToStringAsync("BDDisplayBDCommentsReview", reviews));

            
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "00011"), classifiedBuiness.UseId.ToString());
            
            classifiedBuiness.businessWorkingHours = businessHours;
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "33"), await _renderer.RenderPartialToStringAsync("BDDisplayBDDescription", classifiedBuiness));
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "34"), await _renderer.RenderPartialToStringAsync("BDDisplayBDProducts", productCategories));
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "35"), await _renderer.RenderPartialToStringAsync("BDDisplayWH", classifiedBuiness));
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "36"), await _renderer.RenderPartialToStringAsync("BDDisplayBDCommentsAddReview", Review));
            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", "37"), await _renderer.RenderPartialToStringAsync("BDDisplaySimilarBD", similarBusinesses));

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
                        var Content = classifiedBuiness.BusinessContents.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault();
                        if (Content != null)
                        {
                            var ContentVal = Content.BusinessContentValues.FirstOrDefault();
                            if (ContentVal != null)
                            {
                                FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), ContentVal.ContentValue.ToString());
                            }

                            var LabelValue = " ";
                            var LabelPlusConfig = "0" + col.BusinessTemplateConfigId.ToString();
                            if (BrowserCulture == "en-US")
                            {
                                LabelValue = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn;

                            }
                            else
                            {
                                LabelValue = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr;


                            }
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", LabelPlusConfig), LabelValue);

                        }

                        break;
                    case 3: //Lookup
                            //Get Single Content
                        if (classifiedBuiness.BusinessContents.Where(c => c.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault() != null)
                        {
                            int AC = Convert.ToInt32(classifiedBuiness.BusinessContents.Where(c => c.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault().BusinessContentValues.FirstOrDefault().ContentValue.ToString());
                            var strLookup = _context.BusinessTemplateOptions.Where(c => c.BusinessTemplateConfigId == AC).FirstOrDefault().OptionEn;

                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), strLookup);

                        }
                        else
                        {
                            FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), " ");

                        }
                        var optionValue = " ";
                        var optionPlusConfig = "0" + col.BusinessTemplateConfigId.ToString();
                        if (BrowserCulture == "en-US")
                        {
                            optionValue = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionEn;

                        }
                        else
                        {
                            optionValue = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault().BusinessTemplateFieldCaptionAr;


                        }
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", optionPlusConfig), optionValue);

                        break;
                    
                    case 9:
                        //Photos --> Slider
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BDDisplayBDGalary", classifiedBuiness.BusinessContents.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault()));
                        break;

                    case 10:
                        //video -->
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BDDisplayBDVideo", classifiedBuiness.BusinessContents.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault()));
                        break;



                    case 14:
                        //Map 
                        FormTemplate = FormTemplate.Replace(string.Format("##{0}##", col.BusinessTemplateConfigId), await _renderer.RenderPartialToStringAsync("BDDisplayBDCommentsLocationandcontact", classifiedBuiness.BusinessContents.Where(e => e.BusinessTemplateConfigId == col.BusinessTemplateConfigId).FirstOrDefault()));
                        break;
                    default:
                        break;
                }
            }





           
            return Page();

        }
       
        public async Task<IActionResult> OnPostFavourite([FromBody] int ClassifiedBusinessId)
        {
            bool favouriteflag = false;
            var user = await _userManager.GetUserAsync(User);
            var favourite = _context.FavouriteBusiness.Where(a => a.UserId == user.Id && a.ClassifiedBusinessId == ClassifiedBusinessId).FirstOrDefault();
            if (favourite == null)
            {
                var favouriteobj = new FavouriteBusiness() { ClassifiedBusinessId = ClassifiedBusinessId, UserId = user.Id };

                _context.FavouriteBusiness.Add(favouriteobj);
                favouriteflag = true;
            }
            else
                _context.FavouriteBusiness.Remove(favourite);
            _context.SaveChanges();
            return new JsonResult(favouriteflag);
        }
        public async Task<IActionResult> OnPostAddReviews([FromBody] Review Reviewobj)
        {
            try
            {
                if (Reviewobj != null)
                {

                    Review review = new Review
                    {
                        Rating = Reviewobj.Rating,
                        ClassifiedBusinessId = Reviewobj.ClassifiedBusinessId,
                        ReviewDate = DateTime.Now,
                        Name = Reviewobj.Name,
                        Email = Reviewobj.Email,
                        Title = Reviewobj.Title,
                    };

                    _context.Reviews.Add(review);
                    _context.SaveChanges();
                    var reviewBusinessCount = _context.Reviews.Where(a => a.ClassifiedBusinessId == ClassifiedBusinessId).Count();
                    var sumBusinessRating = _context.Reviews.Where(a => a.ClassifiedBusinessId == ClassifiedBusinessId).Sum(a => a.Rating);
                    var Avg = sumBusinessRating/reviewBusinessCount;
                    var BD = _context.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == Reviewobj.ClassifiedBusinessId).FirstOrDefault();
                    BD.Rating = Avg;
                    _dbContext.Attach(BD).State = EntityState.Modified;
                     _dbContext.SaveChanges();

                    toastNotification.AddSuccessToastMessage("Thank you for your review");

                }
            }
            catch (Exception ex)
            {

                toastNotification.AddErrorToastMessage(ex.Message);
            }
            return new JsonResult(Reviewobj);

        }


        public IActionResult OnGetSingleProductForget(int ProductCategoryId)
        {
            var _lstTemplates = _context.Products.Include(e => e.ProductPrices).Include(e => e.ProductCategory).Where(c => c.ProductCategoryId == ProductCategoryId && c.IsActive == true).Select(
                   i => new {

                       CategoryName = i.ProductCategory.TitleEn.ToLower(),
                       CategoryArabicName = i.ProductCategory.TitleAr.ToLower(),
                       //TemplateName = i.ProductName,
                       TemplateName = i.TitleEn,
                       TemplatePic = i.MainPic,
                       TemplateId = i.ProductId,
                       IsFixed = i.IsFixedPrice,
                       Price = i.Price.Value,
                       PriceList = _context.ProductPrices.Where(e => e.ProductId == i.ProductId).ToList()
                   }).ToList();
            return new JsonResult(_lstTemplates);
        }

    }
}
