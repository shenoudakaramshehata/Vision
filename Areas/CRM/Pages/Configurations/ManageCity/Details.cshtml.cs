using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageCity
{
    public class DetailsModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DetailsModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

      public City City { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var cityDetails = await _context.Cities.FirstOrDefaultAsync(m => m.CityId == id);
            if (cityDetails == null)
            {
                return NotFound();
            }
            else 
            {
                City = cityDetails;
            }
            return Page();
        }
    }
}
