using Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;


namespace Vision.Areas.CRM.Pages.Configurations.WalletSubscriptions
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public ApplicationDbContext _applicationDbContext { get; set; }
        public HttpClient httpClient { get; set; }
       
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public string url { get; set; }
        public List<WalletSubscription> walletSubscriptions { get; set; }
       

        [BindProperty(SupportsGet = true)]
        public bool ArLang { get; set; }

        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            httpClient = new HttpClient();
            _applicationDbContext = applicationDbContext;
            walletSubscriptions = new List<WalletSubscription>();
        }
        public async void OnGet()
        {
            walletSubscriptions = _context.WalletSubscriptions.Include(e=>e.Wallet).Where(e=>e.IsPaid==true).OrderByDescending(e => e.SubscriptionDate.Date).ToList();
           

            url = $"{this.Request.Scheme}://{this.Request.Host}";
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();
            if (BrowserCulture == "en-US")

                ArLang = false;

            else
                ArLang = true;
        }
    }
}
