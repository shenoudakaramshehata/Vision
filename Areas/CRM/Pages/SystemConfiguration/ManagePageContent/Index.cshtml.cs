using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision.Areas.CRM.Pages.SystemConfiguration.ManagePageContent
{
    public class IndexModel : PageModel
    {

        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        [BindProperty]
        public PageContent PageContent { get; set; }
        public List<PageContent> pageContents { set; get; }


        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        public ActionResult OnGet()
        {
            pageContents = _context.PageContents.ToList();

            return Page();

        }

    }
}
