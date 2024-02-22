using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageAdTemplateOption
{
    public class DeleteModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DeleteModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
      public AdTemplateOption AdTemplateOption { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AdTemplateOptions == null)
            {
                return NotFound();
            }

            var adtemplateoption = await _context.AdTemplateOptions.FirstOrDefaultAsync(m => m.AdTemplateOptionId == id);

            if (adtemplateoption == null)
            {
                return NotFound();
            }
            else 
            {
                AdTemplateOption = adtemplateoption;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.AdTemplateOptions == null)
            {
                return NotFound();
            }
            var adtemplateoption = await _context.AdTemplateOptions.FindAsync(id);

            if (adtemplateoption != null)
            {
                AdTemplateOption = adtemplateoption;
                _context.AdTemplateOptions.Remove(AdTemplateOption);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
