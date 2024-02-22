
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Areas.CRM.Pages.SystemConfiguration.UserMessages
{

    #nullable disable
    public class IndexModel : PageModel
    {
        private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;


        public string url { get; set; }


        [BindProperty]
        public Contact contactUs { get; set; }


       


        public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            contactUs = new Contact();
  
        }
        public void OnGet()
        {
           
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }

       
        

        public IActionResult OnGetSingleMessageForView(int ContactId)
        {
            var Result = _context.Contacts.Where(c => c.ContactId == ContactId).FirstOrDefault();
            
            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleMessageForDelete(int ContactId)
        {
            contactUs = _context.Contacts.Where(c => c.ContactId == ContactId).FirstOrDefault();
            return new JsonResult(contactUs);
        }

        public async Task<IActionResult> OnPostDeleteMessage(int ContactId)
        {
            try
            {
                Contact MessageObj = _context.Contacts.Where(e => e.ContactId == ContactId).FirstOrDefault();


                if (MessageObj != null)
                {


                    _context.Contacts.Remove(MessageObj);

                    await _context.SaveChangesAsync();

                    _toastNotification.AddSuccessToastMessage("Message Deleted successfully");

                  
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Something went wrong Try Again");
                }
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return RedirectToPage("/SystemConfiguration/UserMessages/Index");
        }

       
        

    }
}
