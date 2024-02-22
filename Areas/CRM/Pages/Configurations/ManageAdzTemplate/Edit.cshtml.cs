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

namespace Vision.Areas.CRM.Pages.Configurations.ManageAdzTemplate
{
    public class EditModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public EditModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AdTemplateConfig AdTemplateConfig { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AdTemplateConfigs == null)
            {
                return NotFound();
            }

            var adtemplateconfig =  await _context.AdTemplateConfigs.FirstOrDefaultAsync(m => m.AdTemplateConfigId == id);
            if (adtemplateconfig == null)
            {
                return NotFound();
            }
            AdTemplateConfig = adtemplateconfig;
           ViewData["ClassifiedAdsCategoryId"] = new SelectList(_context.ClassifiedAdsCategories, "ClassifiedAdsCategoryId", "ClassifiedAdsCategoryId");
           ViewData["FieldTypeId"] = new SelectList(_context.FieldTypes, "FieldTypeId", "FieldTypeId");
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

            _context.Attach(AdTemplateConfig).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdTemplateConfigExists(AdTemplateConfig.AdTemplateConfigId))
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

        private bool AdTemplateConfigExists(int id)
        {
          return _context.AdTemplateConfigs.Any(e => e.AdTemplateConfigId == id);
        }
    }
}
