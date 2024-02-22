using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageAdTemplateOption
{
    public class CreateModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public CreateModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["AdTemplateConfigId"] = new SelectList(_context.AdTemplateConfigs, "AdTemplateConfigId", "AdTemplateConfigId");
            return Page();
        }

        [BindProperty]
        public AdTemplateOption AdTemplateOption { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AdTemplateOptions.Add(AdTemplateOption);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
