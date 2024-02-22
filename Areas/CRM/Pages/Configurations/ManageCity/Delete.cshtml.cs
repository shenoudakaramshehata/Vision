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
    public class DeleteModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DeleteModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
      public City City { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            var cityDel = await _context.Cities.FirstOrDefaultAsync(m => m.CityId == id);

            if (cityDel == null)
            {
                return NotFound();
            }
            else 
            {
                City = cityDel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }
            var cityDel = await _context.Cities.FindAsync(id);

            if (cityDel != null)
            {
                City = cityDel;
                _context.Cities.Remove(City);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
