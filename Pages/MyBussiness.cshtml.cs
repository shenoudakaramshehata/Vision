using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Pages
{
    public class MyBussinessModel : PageModel
    {
        private CRMDBContext _context;
        public List<ClassifiedBusiness> BDs = new List<ClassifiedBusiness>();

        private UserManager<ApplicationUser> _userManager { get; }
        public static List<ClassifiedBusiness> Listings2 = new List<ClassifiedBusiness>();
        [BindProperty]
        public bool BDActivated  { get; set; }

        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        private IToastNotification _toastNotification { get; }
        public MyBussinessModel(UserManager<ApplicationUser> userManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _context = Context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> OnPostBranchesList([FromBody] int num)
        {
            var user = await _userManager.GetUserAsync(User);
            var alllistings = _context.ClassifiedBusiness.Include(e => e.BusinessCategory).Where(a => a.UseId == user.Id).ToList();

            var start = (num - 1) * 2;
            var end = (num) * 2;
            Listings2 = alllistings.Skip(start).Take(2).ToList();
            BDs = Listings2;
            return new JsonResult(BDs);
        }
        public async Task<IActionResult> OnPostDeleteBD([FromBody] int num)
        {
            try
            {
                var BDObj = _context.ClassifiedBusiness.Where(e => e.ClassifiedBusinessId == num).FirstOrDefault();
                if (BDObj == null)
                {
                    _toastNotification.AddErrorToastMessage("Bussiness Not Found");
                    return new JsonResult(true);
                }


                var AdContentList = _context.BusinessContents.Where(e => e.ClassifiedBusinessId == BDObj.ClassifiedBusinessId).ToList();
                var newAdContentList = AdContentList.Select(e => e.BusinessContentId).ToList();
                if (AdContentList != null)
                {
                    var ContentValues = _context.BusinessContentValues.Where(e => newAdContentList.Contains(e.BusinessContentId)).ToList();
                    _context.BusinessContentValues.RemoveRange(ContentValues);
                }
                _context.BusinessContents.RemoveRange(AdContentList);
                _context.ClassifiedBusiness.Remove(BDObj);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Bussiness Deleted Sucessfully");
                return new JsonResult(true);

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("SomeThing Went Error");
                return new JsonResult(false);
            }
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/identity/account/login");

            }
            if (first)
            {
                BDs = _context.ClassifiedBusiness.Include(e => e.BusinessCategory).Where(a => a.UseId == user.Id).ToList();
                Listings2 = BDs;
                first = false;
            }
            else
                BDs = Listings2;
            var alllistings = _context.ClassifiedBusiness.Include(e => e.BusinessCategory).Where(a => a.UseId == user.Id).ToList();

            float number = (float)alllistings.Count() / 2;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string isActive, long BDId)
        {
            var BD = _context.ClassifiedBusiness.Where(c => c.ClassifiedBusinessId == BDId).FirstOrDefault();
            if (BD == null)
            {
                return Redirect("PageNF");
            }
            try
            {
                bool BDIsActive = false;
                if (isActive== "0")
                {
                    BDIsActive = true;
                }
                
                BD.IsActive = BDIsActive;
                _context.Attach(BD).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                if (BDIsActive == true)
                {
                    _toastNotification.AddSuccessToastMessage("Bussiness Directory Activated");
                }
                else
                {
                    _toastNotification.AddSuccessToastMessage("Bussiness Directory DeActivated");
                }
                
                
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something wnt Error");

            }
            return RedirectToPage("MyBussiness");
        }
    }
}

