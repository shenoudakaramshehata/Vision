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

namespace Vision.Areas.CRM.Pages.Configurations.ManageCategory
{
    public class EditModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public EditModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ClassifiedAdsCategory ClassifiedAdsCategory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.ClassifiedAdsCategories == null)
            {
                return NotFound();
            }

            var classifiedadscategory =  await _context.ClassifiedAdsCategories.FirstOrDefaultAsync(m => m.ClassifiedAdsCategoryId == id);
            if (classifiedadscategory == null)
            {
                return NotFound();
            }
            ClassifiedAdsCategory = classifiedadscategory;
           ViewData["ClassifiedAdsCategoryParentId"] = new SelectList(_context.ClassifiedAdsCategories, "ClassifiedAdsCategoryId", "ClassifiedAdsCategoryId");
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

            _context.Attach(ClassifiedAdsCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassifiedAdsCategoryExists(ClassifiedAdsCategory.ClassifiedAdsCategoryId))
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

        private bool ClassifiedAdsCategoryExists(int id)
        {
          return _context.ClassifiedAdsCategories.Any(e => e.ClassifiedAdsCategoryId == id);
        }
    }
}
