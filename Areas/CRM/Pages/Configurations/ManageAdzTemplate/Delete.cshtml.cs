﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageAdzTemplate
{
    public class DeleteModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DeleteModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        [BindProperty]
      public AdTemplateConfig AdTemplateConfig { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.AdTemplateConfigs == null)
            {
                return NotFound();
            }

            var adtemplateconfig = await _context.AdTemplateConfigs.FirstOrDefaultAsync(m => m.AdTemplateConfigId == id);

            if (adtemplateconfig == null)
            {
                return NotFound();
            }
            else 
            {
                AdTemplateConfig = adtemplateconfig;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.AdTemplateConfigs == null)
            {
                return NotFound();
            }
            var adtemplateconfig = await _context.AdTemplateConfigs.FindAsync(id);

            if (adtemplateconfig != null)
            {
                AdTemplateConfig = adtemplateconfig;
                _context.AdTemplateConfigs.Remove(AdTemplateConfig);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}