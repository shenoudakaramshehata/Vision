using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;


namespace Vision.Pages
{
    public class BDOfferDetailsModel : PageModel
    {
        private CRMDBContext _context;
        public BDOffer BDOfferObj;

        
        public BDOfferDetailsModel(CRMDBContext Context)
        {
            _context = Context;
        }
        

        public async Task<IActionResult> OnGet(int Id)
        {

            BDOfferObj = _context.BDOffers.Include(e=>e.BDOfferImages).Where(a => a.BDOfferId == Id).FirstOrDefault();
                if (BDOfferObj == null)
                {
                    return Redirect("/PageNF");
                }
                return Page();
        }

    }
}
