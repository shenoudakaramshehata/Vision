using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;

namespace Vision.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<ClassifiedAdsCategory> classifiedAdsCategories = new List<ClassifiedAdsCategory>();
        public static List<ClassifiedAdsCategory> StaticClassifiedAdsCategories = new List<ClassifiedAdsCategory>();
        public  List<ClassifiedAdsCategory> CategoriesTitle = new List<ClassifiedAdsCategory>();
        public CategoryModel(CRMDBContext Context, UserManager<ApplicationUser> userManager)
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
            if (user.AvailableClassified == 0)
            {
                return Redirect("/WalletPricing");
            }
            if (categoryId == null)
            {
                classifiedAdsCategories = _context.ClassifiedAdsCategories.Where(e=>e.ClassifiedAdsCategoryParentId==null).ToList();
                StaticClassifiedAdsCategories.Clear();
            }
            else
            {
                classifiedAdsCategories = _context.ClassifiedAdsCategories.Where(e=>e.ClassifiedAdsCategoryParentId==categoryId).ToList();
                CategoriesTitle = StaticClassifiedAdsCategories;
            }
            
            return Page();
        }
        public IActionResult OnGetInfoList(int CatId)
        {

            bool hasChild = _context.ClassifiedAdsCategories.Any(x => x.ClassifiedAdsCategoryParentId == CatId);
            if (hasChild)
            {
               var selectedCategory = _context.ClassifiedAdsCategories.Where(e => e.ClassifiedAdsCategoryId== CatId).FirstOrDefault();
                StaticClassifiedAdsCategories.Add(selectedCategory);
            }
            return new JsonResult(hasChild);
        }
        public IActionResult OnGetRemoveFromStatic(int index)
        {
            if (index == -1)
            {
                StaticClassifiedAdsCategories.Clear();
            }
            else
            {
                for (int i = index + 1; i <= StaticClassifiedAdsCategories.Count; i++)
                {
                    StaticClassifiedAdsCategories.RemoveAt(i);
                }
            }
            
            return new JsonResult(true);
        }
    }
}
