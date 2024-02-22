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

namespace Vision.Areas.TemplatePages.Pages
{
    public class UserProfileInfoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CRMDBContext _context { get; set; }
        
        public  ApplicationUser user { set; get; }
        public  int addLisCount { set; get; }
        public  int userFolwersCount { set; get; }
        public ApplicationDbContext _applicationDbContext { get; set; }
        public UserProfileInfoModel(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, CRMDBContext Context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = Context;
            _applicationDbContext = applicationDbContext;

        }
        public async Task<IActionResult> OnGet(string id)
        {
             user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                RedirectToPage("PageNF");
            }
             addLisCount = _context.AddListings.Where(e => e.CreatedByUser == user.Email).Count();
             userFolwersCount = _context.FolowProfile.Where(e => e.UserId == user.Id).Count();
            return Page(); 
        }
    }
}
