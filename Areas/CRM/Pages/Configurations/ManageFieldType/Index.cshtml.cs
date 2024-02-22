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
    public class IndexModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;

        public IndexModel(Vision.Data.CRMDBContext context)
        {
            _context = context;
        }

        public IList<FieldType> FieldType { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.FieldTypes != null)
            {
                FieldType = await _context.FieldTypes.ToListAsync();
            }
        }
    }
}
