using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace Vision.Areas.CRM.Pages.SystemConfiguration.ManagePageContent
{
    public class EditModel : PageModel
    {

        private CRMDBContext _context;
        private readonly IToastNotification _toastNotification;


        [BindProperty]
        public PageContent PageContent { get; set; }


        public EditModel(CRMDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }


        public ActionResult OnGet(int id)
        {
            PageContent = _context.PageContents.FirstOrDefault(a =>a.PageContentId == id );

            ViewData["ContentEn"] = PageContent.ContentEn;

            return Page();
        }


        public ActionResult OnPost(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _toastNotification.AddErrorToastMessage("Please Enter All Required Data");

                    return Page();
                }

                var page = _context.PageContents.FirstOrDefault(a => a.PageContentId == id);

                if (page is null)
                {
                    _toastNotification.AddErrorToastMessage("Page Content Not Found");

                    return NotFound();
                }

                page.ContentAr = PageContent.ContentAr;

                page.ContentEn = PageContent.ContentEn;

                _context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Page Content Updated successfully");
                
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
            }

           return Redirect("./");
        }
    }
}
