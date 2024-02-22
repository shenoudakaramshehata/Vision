using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;

namespace Vision.Pages
{
    public class BDCategoryModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<BusinessCategory> BDCategories = new List<BusinessCategory>();
        public static List<BusinessCategory> StaticBDCategories = new List<BusinessCategory>();
        public List<BusinessCategory> CategoriesTitle = new List<BusinessCategory>();
        public BDCategoryModel(CRMDBContext Context, UserManager<ApplicationUser> userManager)
        {
            _context = Context;
            _userManager = userManager;
        }
        public async Task<ActionResult> OnGet(int? categoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/account/login");

            }
            
            if (categoryId == null)
            {
                BDCategories = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == null).ToList();
                StaticBDCategories.Clear();
            }
            else
            {
                BDCategories = _context.BusinessCategories.Where(e => e.BusinessCategoryParentId == categoryId).ToList();
                CategoriesTitle = StaticBDCategories;
            }

            return Page();
        }
        public IActionResult OnGetInfoList(int CatId)
        {

            bool hasChild = _context.BusinessCategories.Any(x => x.BusinessCategoryParentId == CatId);
            if (hasChild)
            {
                var selectedCategory = _context.BusinessCategories.Where(e => e.BusinessCategoryId == CatId).FirstOrDefault();
                StaticBDCategories.Add(selectedCategory);
            }
            return new JsonResult(hasChild);
        }
        public IActionResult OnGetRemoveFromStatic(int index)
        {
            if (index == -1)
            {
                StaticBDCategories.Clear();
            }
            else
            {
                for (int i = index + 1; i <= StaticBDCategories.Count; i++)
                {
                    StaticBDCategories.RemoveAt(i);
                }
            }

            return new JsonResult(true);
        }
        
    }
}
