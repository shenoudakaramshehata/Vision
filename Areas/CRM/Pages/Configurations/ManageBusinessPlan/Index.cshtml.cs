using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.Data;
using Vision.Models;


namespace Vision.Areas.CRM.Pages.Configurations.ManageBusinessPlan
{
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        public List<BussinessPlan> Plans { get; set; }
        [BindProperty]
        public BussinessPlan BussinessPlan { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ArLang { get; set; }

        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            BussinessPlan = new BussinessPlan();
            Plans = new List<BussinessPlan>();
        }
        public void OnGet()
        {
            Plans = _context.BussinessPlans.OrderByDescending(e => e.DurationInMonth).ToList();
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();
            if (BrowserCulture == "en-US")

                ArLang = false;

            else
                ArLang = true;
        }


        public IActionResult OnGetSinglePlanForEdit(int BussinessPlanId)
        {
            var Result = _context.BussinessPlans.Where(c => c.BussinessPlanId == BussinessPlanId).FirstOrDefault();
            return new JsonResult(Result);

        }


        public async Task<IActionResult> OnPostEditforBusinessPlan(int BussinessPlanId)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();

            if (!ModelState.IsValid)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("Please Enter All the Required Data");

                else
                    _toastNotification.AddErrorToastMessage("من فضلك ادخل البيانات كاملة");
                return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
            }
            try
            {
                var model = _context.BussinessPlans.Where(c => c.BussinessPlanId == BussinessPlanId).FirstOrDefault();
                if (model == null)
                {
                    if (BrowserCulture == "en-US")

                        _toastNotification.AddErrorToastMessage("BussinessPlan is not Existed");

                    else
                        _toastNotification.AddErrorToastMessage("الخطة غير موجودة");

                    return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
                }


                model.PlanTlEn = BussinessPlan.PlanTlEn;
                model.PlanTlAr = BussinessPlan.PlanTlAr;
                model.Price = BussinessPlan.Price;
                model.DurationInMonth = BussinessPlan.DurationInMonth;
                model.IsActive = BussinessPlan.IsActive;


                var UpdatedPlan = _context.BussinessPlans.Attach(model);
                UpdatedPlan.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                await _context.SaveChangesAsync();
                if (BrowserCulture == "en-US")

                    _toastNotification.AddSuccessToastMessage("Business Plan Edited successfully ");

                else
                    _toastNotification.AddSuccessToastMessage("تم تعديل الخطة بنجاح");


            }
            catch (Exception)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");

                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");

            }
            return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
        }


        public IActionResult OnGetSinglePlanForView(int BussinessPlanId)
        {
            var Result = _context.BussinessPlans.Where(c => c.BussinessPlanId == BussinessPlanId).Select(i => new
            {
                BussinessPlanId = i.BussinessPlanId,
                PlanTlAr = i.PlanTlAr,
                PlanTlEn = i.PlanTlEn,
                Price = i.Price,
                IsActive = i.IsActive,
                DurationInMonth = i.DurationInMonth,

            }).FirstOrDefault();

            return new JsonResult(Result);
        }


        public IActionResult OnGetSinglePlanForDelete(int BussinessPlanId)
        {
            BussinessPlan = _context.BussinessPlans.Where(c => c.BussinessPlanId == BussinessPlanId).FirstOrDefault();
            return new JsonResult(BussinessPlan);
        }


        public async Task<IActionResult> OnPostDeletePlan(int BussinessPlanId)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();
            try
            {
                BussinessPlan BussinessPlanobj = _context.BussinessPlans.Where(e => e.BussinessPlanId == BussinessPlanId).FirstOrDefault();


                if (BussinessPlanobj != null)
                {

                    _context.BussinessPlans.Remove(BussinessPlanobj);
                    await _context.SaveChangesAsync();
                    if (BrowserCulture == "en-US")

                        _toastNotification.AddSuccessToastMessage("Plan Deleted successfully");
                    else
                        _toastNotification.AddSuccessToastMessage("تم مسح الخطة بنجاح");

                }
                else
                {
                    if (BrowserCulture == "en-US")
                        _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    else
                        _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
                }
            }
            catch (Exception)

            {
                if (BrowserCulture == "en-US")
                    _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
            }

            return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
        }


        public IActionResult OnPostAddPlan()
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();

            if (!ModelState.IsValid)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("Please Enter All the Required Data");

                else
                    _toastNotification.AddErrorToastMessage("من فضلك ادخل البيانات كاملة");
                return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
            }
            try
            {

                _context.BussinessPlans.Add(BussinessPlan);
                _context.SaveChanges();
                if (BrowserCulture == "en-US")

                    _toastNotification.AddSuccessToastMessage("Plan added successfully");

                else
                    _toastNotification.AddSuccessToastMessage("تم اضافة الخطة");

            }
            catch (Exception)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");

                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
            }
            return Redirect("/CRM/Configurations/ManageBusinessPlan/Index");
        }
    }
}
