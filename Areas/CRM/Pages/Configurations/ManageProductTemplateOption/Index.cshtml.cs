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


namespace Vision.Areas.CRM.Pages.Configurations.ManageProductTemplateOption
{
    public class IndexModel : PageModel
    {
        private readonly Vision.Data.CRMDBContext _context;
        private readonly IToastNotification _toastNotification;
        private static int configId = 0;
        public IndexModel(Vision.Data.CRMDBContext context, IToastNotification toastNotification)
        {
            _context = context;
            AdTemplateOptionObj = new ProductTemplateOption();
            _toastNotification = toastNotification;
        }

        public IList<ProductTemplateOption> AdTemplateOption { get;set; } = default!;
        [BindProperty]
        public ProductTemplateOption AdTemplateOptionObj { get; set; }
        public async Task OnGetAsync(int id)
        {
            if (_context.AdTemplateOptions != null)
            {
                AdTemplateOption = await _context.ProductTemplateOptions.Where(e=>e.ProductTemplateConfigId==id)
                .Include(a => a.ProductTemplateConfig).ToListAsync();
                configId = id;
            }
        }
        public async Task<IActionResult> OnPostAddOption()
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please Enter Valid Required Data");
                return Redirect($"/CRM/Configurations/ManageProductTemplateOption/index?id={configId}");

            }
            try
            {
                AdTemplateOptionObj.ProductTemplateConfigId = configId;
                _context.ProductTemplateOptions.Add(AdTemplateOptionObj);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Added Successfully");

            }
            catch (Exception )
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Redirect($"/CRM/Configurations/ManageProductTemplateOption/index?id={configId}");
        }
        public IActionResult OnGetSingleOptionForView(int AdTemplateOptionId)
        {
            var Result = _context.ProductTemplateOptions.Where(c => c.ProductTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(Result);
           
        }
        public IActionResult OnGetSingleOptionForDelete(int AdTemplateOptionId)
        {
            AdTemplateOptionObj = _context.ProductTemplateOptions.Where(c => c.ProductTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(AdTemplateOptionObj);
        }
        public async Task<IActionResult> OnPostDeleteOption(int AdTemplateOptionId)
        {
            try
            {
                ProductTemplateOption OptionObj = _context.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == AdTemplateOptionId).FirstOrDefault();


                if (OptionObj != null)
                {


                    _context.ProductTemplateOptions.Remove(OptionObj);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Option Deleted successfully");
                   
                }
               
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return Redirect($"/CRM/Configurations/ManageProductTemplateOption/index?id={configId}");

        }
        public IActionResult OnGetSingleOptionForEdit(int AdTemplateOptionId)
        {
            AdTemplateOptionObj = _context.ProductTemplateOptions.Where(c => c.ProductTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
            return new JsonResult(AdTemplateOptionObj);

        }
        public async Task<IActionResult> OnPostEditOption(int AdTemplateOptionId)
        {
            try
            {
                var model = _context.ProductTemplateOptions.Where(c => c.ProductTemplateOptionId == AdTemplateOptionId).FirstOrDefault();
                if (model == null)
                {
                    return Redirect($"/CRM/Configurations/ManageProductTemplateOption/index?id={configId}");
                }


                model.OptionAr = AdTemplateOptionObj.OptionAr;
                model.OptionEn = AdTemplateOptionObj.OptionEn;
                
                var UpdatedOption = _context.ProductTemplateOptions.Attach(model);
                UpdatedOption.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Option Edited successfully");


            }
            catch (Exception )
            {
                _toastNotification.AddErrorToastMessage("Something went Error");
                
            }
            return Redirect($"/CRM/Configurations/ManageProductTemplateOption/index?id={configId}");
        }
    }
}
