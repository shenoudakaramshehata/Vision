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
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace Vision.Areas.CRM.Pages.Configurations.ManageBusinessTemplateOptions
{
    public class IndexModel : PageModel
    {
        public string? url { get; set; }
        private readonly Vision.Data.CRMDBContext _context;
        private readonly IToastNotification _toastNotification;
        private static int configId = 0;
        public IndexModel(Vision.Data.CRMDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            BusinessTemplateOptionObj = new BusinessTemplateOption();
            _toastNotification = toastNotification;
        }
        [BindProperty]
        public BusinessTemplateConfig BusinessTemplateConfig { get; set; }
        public List<BusinessTemplateOption> BusinessTemplateOptions { get; set; } = default!;
        [BindProperty]
        public BusinessTemplateOption BusinessTemplateOptionObj { get; set; }

            public IActionResult OnGet(int id)
        {
            url = $"{this.Request.Scheme}://{this.Request.Host}";
            BusinessTemplateConfig = _context.BusinessTemplateConfigs.Where(e => e.BusinessTemplateConfigId == id).FirstOrDefault();
            if (_context.AdTemplateOptions != null)
            {
                BusinessTemplateOptions =  _context.BusinessTemplateOptions.Where(e => e.BusinessTemplateConfigId == id)
                .Include(a => a.BusinessTemplateConfig).ToList();
                //configId = id;
                //DELETETemplateConfig();
                //copyoptions();
                
            }
            return Page();
        }

      
        
        public  IActionResult OnPostAddOption(int id)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter Valid Required Data");
                return Redirect($"/CRM/Configurations/ManageBusinessTemplateOptions/index?id={id}");

            }
            try
            {

                BusinessTemplateOptionObj.BusinessTemplateConfigId = id;
                _context.BusinessTemplateOptions.Add(BusinessTemplateOptionObj);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Added Successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect($"/CRM/Configurations/ManageBusinessTemplateOptions/index?id={id}");
        }
        public IActionResult OnGetSingleOptionForView(int BusinessTemplateOptionId)
        {
            var Result = _context.BusinessTemplateOptions.Where(c => c.BusinessTemplateOptionId == BusinessTemplateOptionId).FirstOrDefault();
            return new JsonResult(Result);
        }
        public IActionResult OnGetSingleOptionForDelete(int BusinessTemplateOptionId)
        {
            BusinessTemplateOptionObj = _context.BusinessTemplateOptions.Where(c => c.BusinessTemplateOptionId == BusinessTemplateOptionId).FirstOrDefault();
            return new JsonResult(BusinessTemplateOptionObj);
        }
        public async Task<IActionResult> OnPostDeleteOption(int BusinessTemplateOptionId)
        {
            try
            {
                BusinessTemplateOption OptionObj = _context.BusinessTemplateOptions.Where(e => e.BusinessTemplateOptionId == BusinessTemplateOptionId).FirstOrDefault();


                if (OptionObj != null)
                {


                    _context.BusinessTemplateOptions.Remove(OptionObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Option Deleted successfully");

                }

            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return Redirect($"/CRM/Configurations/ManageBusinessTemplateOptions/index?id={configId}");

        }
        public IActionResult OnGetSingleOptionForEdit(int BusinessTemplateOptionId)
        {
            BusinessTemplateOptionObj = _context.BusinessTemplateOptions.Where(c => c.BusinessTemplateOptionId == BusinessTemplateOptionId).FirstOrDefault();
            return new JsonResult(BusinessTemplateOptionObj);

        }
        public async Task<IActionResult> OnPostEditOption(int BusinessTemplateOptionId)
        {
            try
            {
                var model = _context.BusinessTemplateOptions.Where(c => c.BusinessTemplateOptionId == BusinessTemplateOptionId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect($"/CRM/Configurations/ManageBusinessTemplateOptions/index?id={configId}");
                }


                model.OptionAr = BusinessTemplateOptionObj.OptionAr;
                model.OptionEn = BusinessTemplateOptionObj.OptionEn;

                var UpdatedOption = _context.BusinessTemplateOptions.Attach(model);
                UpdatedOption.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Edited successfully");


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went Error");

            }
            return Redirect($"/CRM/Configurations/ManageBusinessTemplateOptions/index?id={configId}");
        }
    }
}
