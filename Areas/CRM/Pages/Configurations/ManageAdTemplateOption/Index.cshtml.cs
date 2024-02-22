using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Vision.Data;
using Vision.Models;
using NToastNotify;


namespace Vision.Areas.CRM.Pages.Configurations.ManageAdTemplateOption
{
    public class IndexModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;
        private readonly IToastNotification _toastNotification;
        private static int configId = 0;
        public IndexModel(Vision.Data.CRMDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            AdTemplateOptionObj = new AdTemplateOption();
            _toastNotification = toastNotification;
        }

        public IList<AdTemplateOption> AdTemplateOption { get;set; } = default!;
        [BindProperty]
        public AdTemplateOption AdTemplateOptionObj { get; set; }
        public async Task OnGetAsync(int id)
        {
            if (_context.AdTemplateOptions != null)
            {
                AdTemplateOption = await _context.AdTemplateOptions.Where(e=>e.AdTemplateConfigId==id)
                .Include(a => a.AdTemplateConfig).ToListAsync();
                configId = id;
            }
        }
        public async Task<IActionResult> OnPostAddOption()
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter Valid Required Data");
                return Redirect($"/CRM/Configurations/ManageAdTemplateOption/index?id={configId}");

            }
            try
            {
                AdTemplateOptionObj.AdTemplateConfigId = configId;
                _context.AdTemplateOptions.Add(AdTemplateOptionObj);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Added Successfully");

            }
            catch (Exception )
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect($"/CRM/Configurations/ManageAdTemplateOption/index?id={configId}");
        }
        public IActionResult OnGetSingleOptionForView(int AdTemplateOptionId)
        {
            var Result = _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public IActionResult OnGetSingleOptionForDelete(int AdTemplateOptionId)
        {
            AdTemplateOptionObj = _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(AdTemplateOptionObj);
        }
        public async Task<IActionResult> OnPostDeleteOption(int AdTemplateOptionId)
        {
            try
            {
                AdTemplateOption OptionObj = _context.AdTemplateOptions.Where(e => e.AdTemplateOptionId == AdTemplateOptionId).FirstOrDefault();


                if (OptionObj != null)
                {


                    _context.AdTemplateOptions.Remove(OptionObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Option Deleted successfully");
                   
                }
               
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return Redirect($"/CRM/Configurations/ManageAdTemplateOption/index?id={configId}");

        }
        public IActionResult OnGetSingleOptionForEdit(int AdTemplateOptionId)
        {
            AdTemplateOptionObj = _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(AdTemplateOptionObj);

        }
        public async Task<IActionResult> OnPostEditOption(int AdTemplateOptionId)
        {
            try
            {
                var model = _context.AdTemplateOptions.Where(c => c.AdTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect($"/CRM/Configurations/ManageAdTemplateOption/index?id={configId}");
                }


                model.OptionAr = AdTemplateOptionObj.OptionAr;
                model.OptionEn = AdTemplateOptionObj.OptionEn;
                
                var UpdatedOption = _context.AdTemplateOptions.Attach(model);
                UpdatedOption.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Edited successfully");


            }
            catch (Exception )
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
                
            }
            return Redirect($"/CRM/Configurations/ManageAdTemplateOption/index?id={configId}");
        }
    }
}
