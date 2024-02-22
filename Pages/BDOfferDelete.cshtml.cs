using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using NToastNotify;

namespace Vision.Pages
{
    public class BDOfferDeleteModel : PageModel
    {
        private CRMDBContext _context;
        public BDOffer BDOfferObj;
        private readonly IToastNotification _toastNotification;
        public static long BDId = 0;
        public BDOfferDeleteModel(CRMDBContext Context, IToastNotification toastNotification)
        {
            _context = Context;
            _toastNotification = toastNotification;
        }


        public async Task<IActionResult> OnGet(int Id)
        {

            BDOfferObj = _context.BDOffers.Include(e => e.BDOfferImages).Where(a => a.BDOfferId == Id).FirstOrDefault();
            if (BDOfferObj == null)
            {
                return Redirect("/PageNF");
            }
            BDId = BDOfferObj.ClassifiedBusinessId;
            return Page();
        }
        public async Task<IActionResult> OnPost(int BDOfferId)
        {
            try
            {
                BDOfferObj = _context.BDOffers.Include(e => e.BDOfferImages).Where(a => a.BDOfferId == BDOfferId).FirstOrDefault();
                if (BDOfferObj != null)
                {
                    _context.BDOffers.Remove(BDOfferObj);
                    _context.SaveChanges();
                    _toastNotification.AddSuccessToastMessage("Offer Deleted Successfully");
                }

            }
            catch(Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error..");
            }

            return Redirect($"/BDOffers?BDId={BDId}");
        }

    }
}

