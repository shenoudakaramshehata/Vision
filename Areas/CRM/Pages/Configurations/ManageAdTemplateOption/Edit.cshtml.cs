using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageAdTemplateOption
{
    public class EditModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public EditModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AdTemplateOption AdTemplateOption { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AdTemplateOptions == null)
            {
                return NotFound();
            }

            var adtemplateoption =  await _context.AdTemplateOptions.FirstOrDefaultAsync(m => m.AdTemplateOptionId == id);
            if (adtemplateoption == null)
            {
                return NotFound();
            }
            AdTemplateOption = adtemplateoption;
           ViewData["AdTemplateConfigId"] = new SelectList(_context.AdTemplateConfigs, "AdTemplateConfigId", "AdTemplateConfigId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(AdTemplateOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdTemplateOptionExists(AdTemplateOption.AdTemplateOptionId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AdTemplateOptionExists(int id)
        {
          return _context.AdTemplateOptions.Any(e => e.AdTemplateOptionId == id);
        }
    }
}
