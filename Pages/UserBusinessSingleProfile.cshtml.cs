using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Microsoft.EntityFrameworkCore;

namespace Vision.Pages
{
    public class UserBusinessSingleProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IToastNotification _toastNotification;
        private static string UserprofileId;
        public List<ClassifiedBusiness> ClassifiedBusinessList { get; set; }
        public CRMDBContext _context { get; set; }
        public ClassifiedBusiness ClassifiedBusiness { get; set; }
        public ApplicationUser user { set; get; }
        public int BusinessCount { set; get; }
        public int userFolwersCount { set; get; }
        public FolowProfile folowProfile { set; get; }
        public ApplicationDbContext _applicationDbContext { get; set; }
        public UserBusinessSingleProfileModel(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = Context;
            _applicationDbContext = applicationDbContext;
            _toastNotification = toastNotification;
            user = new ApplicationUser();
        }
        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                //user = await _userManager.FindByIdAsync(id);
                //if (user == null)
                //{
                //    RedirectToPage("PageNF");

                //}
                //var Currentuser = await _userManager.GetUserAsync(User);
                //if (Currentuser == null)
                //{
                //    return Redirect("/Identity/Account/login");

                //}
                ClassifiedBusinessList = await _context.ClassifiedBusiness.Where(a => a.IsActive == true && a.UseId == id).OrderByDescending(e=>e.PublishDate).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).Take(4).ToListAsync();
                ClassifiedBusiness = _context.ClassifiedBusiness.Where(a => a.IsActive == true && a.UseId == id).OrderByDescending(e => e.PublishDate).Include(a => a.BusinessCategory).ThenInclude(a => a.BusinessTemplateConfigs).FirstOrDefault();

                BusinessCount = _context.ClassifiedBusiness.Where(e => e.UseId == user.Id).Count();
                userFolwersCount = _context.FolowProfile.Where(e => e.UserId == user.Id).Count();
                //folowProfile = _context.FolowProfile.Where(e => e.UserId == user.Id && e.Id == Currentuser.Id).FirstOrDefault();
                UserprofileId = id;
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostFolowToProfile([FromBody] int UserId)
        {
            bool UserIsFolowFlag = false;
            var user = await _userManager.GetUserAsync(User);
            var FolowProfile = _context.FolowProfile.Where(a => a.UserId == user.Id && a.Id == UserprofileId).FirstOrDefault();
            if (FolowProfile == null)
            {
                var FolowProfileObj = new FolowProfile() { Id = UserprofileId, UserId = user.Id };
                _context.FolowProfile.Add(FolowProfileObj);
                UserIsFolowFlag = true;
            }
            else
                _context.FolowProfile.Remove(FolowProfile);
            _context.SaveChanges();
            return new JsonResult(UserIsFolowFlag);
        }
    }
}
