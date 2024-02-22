using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Vision.Data;
using Vision.Models;
using Vision.ViewModels;

namespace Vision.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }

        public ShoppingCartModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }
        public double cartSubTotal { get; set; }
        [BindProperty]
        public List<ShoppingCart> cartProducts { get; set; }
        [BindProperty]
        public int productQuantity { get; set; }
        public ShoppingCart? cartShippingCost { get; set; }
        public List<ShopingCartProductExtraFeatures> productExtraFeatures { get; set; }
        public double totalCost { get; set; }

        public ApplicationUser user { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");

                }

                cartProducts = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                     .Include(e => e.Product)
                                                     .Include(e => e.ProductPrice)
                                                     .Include(e => e.ShopingCartProductExtraFeatures)
                                                     .ThenInclude(e => e.ProductExtra)
                                                     .ToList();

                
                foreach (var product in cartProducts)
                {
                    cartSubTotal += product.ProductTotal;
                }

                cartShippingCost = await _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                                          .Include(e => e.Product)
                                                                          .ThenInclude(s => s.ProductCategory)
                                                                          .ThenInclude(s => s.ClassifiedBusiness)
                                                                          .FirstOrDefaultAsync();
                if(cartShippingCost != null) {
                    totalCost = cartSubTotal + cartShippingCost.Product.ProductCategory.ClassifiedBusiness.Deliverycost;

                }
                else { totalCost = 0; }
                return Page();
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);

            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteItemFromCart(int ShoppingCartId)
        {
            var user = await _userManager.GetUserAsync(User);
            var shoppingcartToDelete = _context.ShoppingCarts
                                                .Where(s => s.ShoppingCartId == ShoppingCartId && s.UserId == user.Id)
                                                .Include(e => e.ShopingCartProductExtraFeatures)
                                                .Include(e => e.ProductPrice)
                                                .FirstOrDefault();
            if (shoppingcartToDelete == null)
            {
                _toastNotification.AddErrorToastMessage("Shopping Cart Item not exist");
                return Page();
            }
            if (shoppingcartToDelete.ShopingCartProductExtraFeatures.Count > 0)
            {
                var cartExtra = _context.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == shoppingcartToDelete.ShoppingCartId).FirstOrDefault();
                _context.ShopingCartProductExtraFeatures.Remove(cartExtra);
            }
            _context.ShoppingCarts.Remove(shoppingcartToDelete);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Cart Item deleted Successfully");
            return RedirectToPage("/ShoppingCart");
        }

        public async Task<IActionResult> OnPostUpdateItemQuantity([FromBody] List<ShoppingCart> ItemQuantityVmList)
        {
                var status = false;
                var user = await _userManager.GetUserAsync(User);
                   if (ItemQuantityVmList != null)
                    {
                        foreach (var item in ItemQuantityVmList)
                        {
                            var itemObj = _context.ShoppingCarts
                                        .Where(e => e.ShoppingCartId == item.ShoppingCartId && e.UserId == user.Id)
                                        .FirstOrDefault();
                                if(itemObj.ProductQty != item.ProductQty)
                                {
                                    itemObj.ProductQty = item.ProductQty;
                                    itemObj.ProductTotal = itemObj.ItemPrice * itemObj.ProductQty;
                                    _context.Attach(itemObj).State = EntityState.Modified;
                                    _context.SaveChanges();
                                    status = true;
                                }
                            
                        }
                _toastNotification.AddSuccessToastMessage("Cart Item Quantity Successfully");
                return new JsonResult(status);
                    }
            return new JsonResult(status);

     
        }

    }
}