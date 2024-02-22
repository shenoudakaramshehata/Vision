using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageFieldType
{
    public class DeleteModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DeleteModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FieldType FieldType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.FieldTypes == null)
            {
                return NotFound();
            }

            var fieldtype = await _context.FieldTypes.FirstOrDefaultAsync(m => m.FieldTypeId == id);

            if (fieldtype == null)
            {
                return NotFound();
            }
            else 
            {
                FieldType = fieldtype;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.FieldTypes == null)
            {
                return NotFound();
            }
            var fieldtype = await _context.FieldTypes.FindAsync(id);

            if (fieldtype != null)
            {
                FieldType = fieldtype;
                _context.FieldTypes.Remove(FieldType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
