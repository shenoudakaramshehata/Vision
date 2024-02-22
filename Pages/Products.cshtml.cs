using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Vision.Pages
{
    public class ProductsModel : PageModel
    {
        private CRMDBContext _context;
        public List<Product> ProductsList = new List<Product>();

        private UserManager<ApplicationUser> _userManager { get; }
        public static List<Product> Listings2 = new List<Product>();

        public List<int> Pagenumbers = new List<int>();
        public static bool first = true;
        public static long BussinessDirId = 0;
        private IToastNotification _toastNotification { get; }
        public ProductsModel(UserManager<ApplicationUser> userManager, CRMDBContext Context, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _context = Context;
            _toastNotification = toastNotification;
        }
        
        public async Task<IActionResult> OnPostBranchesList([FromBody] int num)
        {
            var user = await _userManager.GetUserAsync(User);
            var alllistings = _context.Products.Include(e => e.ProductCategory).ThenInclude(e => e.ClassifiedBusiness).Where(a => a.ProductCategory.ClassifiedBusinessId == BussinessDirId).ToList();

            var start = (num - 1) * 2;
            var end = (num) * 2;
            Listings2 = alllistings.Skip(start).Take(2).ToList();
            ProductsList = Listings2;
            return new JsonResult(ProductsList);
        }
        public async Task<IActionResult> OnPostDeleteProduct([FromBody] int num)
        {
            try
            {
                var ProductObj = _context.Products.Where(e => e.ProductId == num).FirstOrDefault();
                if (ProductObj == null)
                {
                    _toastNotification.AddErrorToastMessage("Product Not Found");
                    return new JsonResult(BussinessDirId);
                }

                var pricesList = _context.ProductPrices.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                if (pricesList != null)
                {
                    _context.ProductPrices.RemoveRange(pricesList);
                }
                var extraList = _context.ProductExtras.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                if (extraList != null)
                {
                    _context.ProductExtras.RemoveRange(extraList);
                }
                var AdContentList = _context.ProductContents.Where(e => e.ProductId == ProductObj.ProductId).ToList();
                var newAdContentList = AdContentList.Select(e => e.ProductContentId).ToList();
                if (AdContentList != null)
                {
                    var ContentValues = _context.ProductContentValues.Where(e => newAdContentList.Contains(e.ProductContentId)).ToList();
                    _context.ProductContentValues.RemoveRange(ContentValues);
                }
                _context.ProductContents.RemoveRange(AdContentList);
                _context.Products.Remove(ProductObj);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Product Deleted Sucessfully");
                return new JsonResult(BussinessDirId);

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("SomeThing Went Error");
                return new JsonResult(BussinessDirId);
            }
        }
        public async Task<IActionResult> OnGet(long BDId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/identity/account/login");

            }
            //var BDCheck=_context.ClassifiedBusiness.Where(a => a.UseId == user.Id && a.ClassifiedBusinessId== BDId).FirstOrDefault();

            //if (BDCheck==null) 
            //{
            //    return Redirect("/PageNF");
            //}
            BussinessDirId = BDId;
            if (first)
            {
                ProductsList = _context.Products.Include(e => e.ProductCategory).ThenInclude(e=>e.ClassifiedBusiness).Where(a => a.ProductCategory.ClassifiedBusinessId== BDId).ToList();
                Listings2 = ProductsList;
                first = false;
            }
            else
                ProductsList = Listings2;
            var alllistings = _context.Products.Include(e => e.ProductCategory).ThenInclude(e => e.ClassifiedBusiness).Where(a => a.ProductCategory.ClassifiedBusinessId == BDId).ToList();

            float number = (float)alllistings.Count() / 2;
            var pagenumber = Math.Ceiling(number);
            for (int i = 1; i <= pagenumber; i++)
            {
                Pagenumbers.Add(i);
            }
            return Page();
        }
    }
}
