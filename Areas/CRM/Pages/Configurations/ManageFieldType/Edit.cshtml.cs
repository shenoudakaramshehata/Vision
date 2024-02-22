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

namespace Vision.Areas.CRM.Pages.Configurations.ManageFieldType
{
    public class EditModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public EditModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FieldType FieldType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FieldTypes == null)
            {
                return NotFound();
            }

            var fieldtype =  await _context.FieldTypes.FirstOrDefaultAsync(m => m.FieldTypeId == id);
            if (fieldtype == null)
            {
                return NotFound();
            }
            FieldType = fieldtype;
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

            _context.Attach(FieldType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldTypeExists(FieldType.FieldTypeId))
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

        private bool FieldTypeExists(int id)
        {
          return _context.FieldTypes.Any(e => e.FieldTypeId == id);
        }
    }
}
