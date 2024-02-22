using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageCategory
{
    public class DetailsModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DetailsModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

      public ClassifiedAdsCategory ClassifiedAdsCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ClassifiedAdsCategories == null)
            {
                return NotFound();
            }

            var classifiedadscategory = await _context.ClassifiedAdsCategories.FirstOrDefaultAsync(m => m.ClassifiedAdsCategoryId == id);
            if (classifiedadscategory == null)
            {
                return NotFound();
            }
            else 
            {
                ClassifiedAdsCategory = classifiedadscategory;
            }
            return Page();
        }
    }
}
