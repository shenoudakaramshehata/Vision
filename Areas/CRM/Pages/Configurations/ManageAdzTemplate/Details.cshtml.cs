using System;
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
    public class DetailsModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public DetailsModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

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
    }
}
