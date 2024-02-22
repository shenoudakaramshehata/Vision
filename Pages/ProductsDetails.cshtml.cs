using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using Vision.Data;
using Vision.Models;
using Vision.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Vision.ViewModels;
using NToastNotify;
using System.Globalization;

namespace Vision.Pages
{
    public class ProductsDetailsModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IRazorPartialToStringRenderer _renderer;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public readonly IToastNotification toastNotification;
        public HttpClient httpClient { get; set; }        
        private readonly UserManager<ApplicationUser> _userManager;
        public IRequestCultureFeature locale;
        public string BrowserCulture;
        [BindProperty]
        public ShoppingCart shoppingCart { get; set; }
        public string strcheckboxlist { get; set; }
        [BindProperty]
        public int QUtty { get; set; }
        public ApplicationUser user { get; set; }
        public double cartSubTotal { get; set; }
        
      public double productPrice { get; set; }

        public string productTitle { get; set; }

        
        public List<ShoppingCart> cartProducts { get; set; }
        #region ProductObj
        public Product product { get; set; }
        #endregion
        #region ExtraFeatures
        public List<ProductExtra> productExtrasList { get; set; }
        #endregion
        #region ProductId
        [BindProperty]
        public long ProductId { get; set; }
        #endregion
        #region Product Prices
        [BindProperty]
        public List<ProductPrice> productPrices { get; set; }
        #endregion

        public ProductPrice prodcutPrice { get; set; }
    #region constructor
    public ProductsDetailsModel(IToastNotification _toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment, IRazorPartialToStringRenderer renderer, CRMDBContext Context, ApplicationDbContext dbContext)
        {
            _context = Context;
            _dbContext = dbContext;
            toastNotification = _toastNotification;
            _renderer = renderer;
            httpClient = new HttpClient();
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            product = new Product();


        }
        #endregion
        #region OnGet
        //public async Task<IActionResult> OnGet(long id)
        public async Task<IActionResult> OnGet()
        {            
            //#region Product Object
            //product = _context.Products.Include(e=>e.ProductPrices).Where(e=>e.ProductId == id).FirstOrDefault();
            //#endregion
            //#region ProductId
            //ProductId = product.ProductId;
            //#endregion
            //#region Product Extras List
            //productExtrasList = _context.ProductExtras.Where(e=>e.ProductId==product.ProductId).ToList();
            //#endregion
            //#region productPrices
            //productPrices = _context.ProductPrices.Where(e => e.ProductId == product.ProductId).ToList();
            //#endregion

            //var user = await _userManager.GetUserAsync(User);

            //cartProducts = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
            //                                                     .Include(e => e.Product).Include(e=>e.ProductPrice)
            //                                                     .ToList();
            //foreach (var productTotalPrice in cartProducts)
            //{
            //    productTitle = productTotalPrice.Product.IsFixedPrice == true ? productTotalPrice.Product.ProductName : productTotalPrice.ProductPrice.ProductPriceTilteEn;

            //    productPrice = productTotalPrice.Product.IsFixedPrice == true ? productTotalPrice.ItemPrice : productTotalPrice.ProductPrice.Price;

            //    cartSubTotal += productTotalPrice.ProductTotal;
            //     strcheckboxlist = String.Join(", ", _context.ShopingCartProductExtraFeatures.Include(e=>e.ProductExtra).Where(c => c.ShoppingCartId == productTotalPrice.ShoppingCartId).Select(c => c.ProductExtra.ExtraTitleEn).ToArray());
            //}
            return Page();
        }
        #endregion

        #region ProductDetails
        public IActionResult OnPostProductDetails([FromBody]int num)
        {

            var ClassifiedAd = _context.Products.Where(c => c.ProductId == num).Include(c => c.ProductContents).ThenInclude(c => c.ProductContentValues).Select(c => new
            {
                ClassifiedAdId = c.ProductId,
                ClassifiedAdsCategoryId = c.ProductCategoryId,
                IsActive = c.IsActive,
                PublishDate = c.MainPic,
                //UseId = c.ProductName,
                Views = c.Price,
                ProductTypeId = c.ProductTypeId,

                ProductContents = c.ProductContents.Select(l => new
                {
                    ProductContentId = l.ProductContentId,
                    ProductTemplateConfigId = l.ProductTemplateConfigId,
                    FieldTypeId = _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId,
                    ProductTemplateFieldCaptionAr = _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionAr,
                    ProductTemplateFieldCaptionEn = _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().ProductTemplateFieldCaptionEn,

                    ProductContentValues = l.ProductContentValues.Select(k => new
                    {
                        ProductContentValueId = k.ProductContentValueId,
                        //ContentValue = k.ContentValue,
                        ContentValue = _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 3 || _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 6 || _context.ProductTemplateConfigs.Where(e => e.ProductTemplateConfigId == l.ProductTemplateConfigId).FirstOrDefault().FieldTypeId == 13 ? _context.ProductTemplateOptions.Where(e => e.ProductTemplateOptionId == Convert.ToInt32(k.ContentValue)).FirstOrDefault().OptionEn : k.ContentValue,

                    }).ToList()


                }).ToList(),

            }).FirstOrDefault();

            return new JsonResult(ClassifiedAd);
        }

        #endregion
        #region OnPost
        public async Task<IActionResult> OnPost(long id)
        {
            try
            {
                productExtrasList = _context.ProductExtras.Where(e => e.ProductId == product.ProductId).ToList();
                productPrices = _context.ProductPrices.Where(e => e.ProductId == product.ProductId).ToList();
                int Quantity = int.Parse(Request.Form["quantity"]);

                var user = await _userManager.GetUserAsync(User);

                if (user is null)
                {
                    toastNotification.AddErrorToastMessage("Must Be Login First");

                    return Redirect("/identity/account/login");

                }

                var Product = _context.Products.Include(i => i.ProductCategory.ClassifiedBusiness).Where(e => e.ProductId == id).FirstOrDefault();

                var itemAlreadyExistInCustomerCart =
                       _context.ShoppingCarts.FirstOrDefault(s =>
                       s.ProductId == Product.ProductId &&
                       s.UserId == user.Id);

                var shopDeliveryCost = Product.ProductCategory.ClassifiedBusiness.Deliverycost;

                if (shopDeliveryCost == null)
                {
                    shopDeliveryCost = 0;
                }

                ShoppingCart shoppingItemObj = new ShoppingCart();
                int ProductPriceId = 0;

                var checkbox = Request.Form["ProductPriceActiveId"];

                if (checkbox == "on")
                {

                    ProductPriceId = int.Parse(Request.Form["ProductPriceId"]);
                    var productPrice = _context.ProductPrices.Where(e => e.ProductPriceId == ProductPriceId).FirstOrDefault().Price;
                    prodcutPrice = _context.ProductPrices.Where(e => e.ProductPriceId == ProductPriceId).FirstOrDefault();
                    //var productsPrice= Request.Form["Price"];
                }



                if (itemAlreadyExistInCustomerCart != null)
                {
                    var newQnt = itemAlreadyExistInCustomerCart.ProductQty;
                    itemAlreadyExistInCustomerCart.ProductQty += Quantity;
                    itemAlreadyExistInCustomerCart.ProductTotal = (itemAlreadyExistInCustomerCart.ProductTotal * (newQnt + 1)) / (newQnt);
                    itemAlreadyExistInCustomerCart.ItemPrice = Product.Price.Value;
                    itemAlreadyExistInCustomerCart.ProductPriceId = ProductPriceId != 0 ? ProductPriceId : null;
                    itemAlreadyExistInCustomerCart.Deliverycost = shopDeliveryCost;
                    _context.Attach(itemAlreadyExistInCustomerCart).State = EntityState.Modified;
                    _context.SaveChanges();

                    if (itemAlreadyExistInCustomerCart.ShopingCartProductExtraFeatures != null)
                    {
                        foreach (var shopcartVm in itemAlreadyExistInCustomerCart.ShopingCartProductExtraFeatures.ToList())
                        {
                            var shopcart = new ShopingCartProductExtraFeatures()
                            {

                                Price = shopcartVm.Price,
                                ProductId = shopcartVm.ProductId,
                                ProductExtraId = shopcartVm.ProductExtraId,
                                ShoppingCartId = itemAlreadyExistInCustomerCart.ShoppingCartId
                            };
                            _context.ShopingCartProductExtraFeatures.Add(shopcart);
                        }

                        _context.SaveChanges();
                    }

                    toastNotification.AddSuccessToastMessage("Item Already in Cart");

                    return RedirectToPage("/ProductsDetails", new { id });
                }

                shoppingItemObj = new ShoppingCart
                {
                    UserId = user.Id,
                    ProductId = Product.ProductId,
                    ItemPrice = Product.Price.Value,
                    ProductQty = Quantity,
                    ProductTotal = Product.IsFixedPrice == true ? Product.Price.Value * Quantity : prodcutPrice.Price * Quantity,
                    Deliverycost = shopDeliveryCost,
                    ProductPriceId = ProductPriceId != 0 ? ProductPriceId : null,

                };
                _context.ShoppingCarts.Add(shoppingItemObj);
                _context.SaveChanges();

                List<ShopingCartProductExtraFeatures> shopingCartProductExtraFeatures = new List<ShopingCartProductExtraFeatures>();
                var extrafeatureslist = _context.ProductExtras.Where(e => e.ProductId == Product.ProductId).ToList();
                if (extrafeatureslist != null)
                {
                    if (extrafeatureslist.Count > 0)
                    {
                        foreach (var item in extrafeatureslist)
                        {
                            var extrafeaturesTitle = item.ExtraTitleEn;
                            int extrafeaturesId = item.ProductExtraId;

                            var extraFeaturesCheckBox = Request.Form[extrafeaturesTitle];
                            if (extraFeaturesCheckBox == "on")
                            {
                                var ProductExtraId = int.Parse(Request.Form[extrafeaturesId.ToString()]);
                                var ExtraProductPrice = _context.ProductExtras.Where(e => e.ProductExtraId == ProductExtraId).FirstOrDefault();
                                var shopcart = new ShopingCartProductExtraFeatures()
                                {
                                    Price = ExtraProductPrice.Price,
                                    ProductId = ExtraProductPrice.ProductId,
                                    ProductExtraId = ExtraProductPrice.ProductExtraId,
                                    ShoppingCartId = shoppingItemObj.ShoppingCartId
                                };
                                _context.ShopingCartProductExtraFeatures.Add(shopcart);
                                _context.SaveChanges();

                                shoppingItemObj.ProductTotal = Product.IsFixedPrice == true ? (Product.Price.Value * Quantity) + shopcart.Price : (prodcutPrice.Price * Quantity) + shopcart.Price;
                                _context.Attach(shoppingItemObj).State = EntityState.Modified;
                                _context.SaveChanges();

                            }
                        }
                    }
                }


                toastNotification.AddSuccessToastMessage("Item added in Cart");

                return RedirectToPage("/ProductsDetails", new { id });
            }
            catch (Exception e)
            {
                toastNotification.AddErrorToastMessage(e.Message);
                return RedirectToPage("/ProductsDetails", new { id });
            }
        }
        #endregion
        public async Task<IActionResult> OnPostDeleteItemFromCart(int ShoppingCartId)
        {
            var shoppingcartToDelete = _context.ShoppingCarts.Where(s => s.ShoppingCartId == ShoppingCartId).FirstOrDefault();
            if (shoppingcartToDelete != null)
            {
                toastNotification.AddErrorToastMessage("Shopping Cart Item not exist");
                return Page();
            }

            if (shoppingcartToDelete.ShopingCartProductExtraFeatures != null)
            {
                var shoppingcartProductextraFeatures = _context.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == shoppingcartToDelete.ShoppingCartId).FirstOrDefault();
                _context.ShopingCartProductExtraFeatures.Remove(shoppingcartProductextraFeatures);
                _context.SaveChanges();
            }

            _context.ShoppingCarts.Remove(shoppingcartToDelete);
            _context.SaveChanges();
            toastNotification.AddErrorToastMessage("Cart Item deleted Successfully");
            return Page();
        }
        //public async Task<IActionResult> OnPostAddToCart(int ItemId)
        //{
        //    bool Islogin = false;
        //    bool OutOfStock = false;

        //    try
        //    {
        //        int Quantity = 1;

        //        var user = await _userManager.GetUserAsync(User);

        //        if (user is null)
        //        {
        //            _toastNotification.AddErrorToastMessage("Must Be Login First");

        //            return new JsonResult(Islogin);

        //        }

        //        Islogin = true;

        //        var UserId = _context.CustomerNs.Where(a => a.Email == user.Email)
        //                        .FirstOrDefault().CustomerId;


        //        var UserIpAddress = GetUserIpAddress();

        //        var country = GetUserCountryByIp(UserIpAddress);

        //        string CountryToUse;

        //        if (country != null)
        //        {
        //            var DbCountry = _context.Country.Any(i => i.CountryTlen == country);

        //            CountryToUse = country;

        //            if (!DbCountry)
        //            {
        //                var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
        //                CountryToUse = firstCountry;
        //            }


        //        }
        //        else
        //        {
        //            var firstCountry = _context.Country.FirstOrDefault().CountryTlen;
        //            CountryToUse = firstCountry;
        //        }

        //        var DbUserCart = await _context.ShoppingCart.AnyAsync(a => a.ItemId == ItemId
        //                                       && a.CustomerId == UserId);

        //        OutOfStock = _context.Item.Where(i => i.ItemId == ItemId).FirstOrDefault().OutOfStock;


        //        if (DbUserCart)
        //        {

        //            _toastNotification.AddSuccessToastMessage("Item exist in cart");

        //            return new JsonResult(Islogin);

        //            //var UserItem = await _context.ShoppingCart
        //            //                        .FirstOrDefaultAsync(a => a.ItemId == ItemId && a.CustomerId==UserId);

        //            //UserItem.ItemQty = Quantity;

        //            //UserItem.ItemTotal = Quantity * UserItem.ItemPrice;

        //            //_context.SaveChanges();

        //            //_toastNotification.AddSuccessToastMessage("Item added to cart");

        //        }
        //        else
        //        {
        //            var Item = _context.Item.Where(e => e.ItemId == ItemId).FirstOrDefault();


        //            if (Item is null)
        //            {
        //                _toastNotification.AddSuccessToastMessage("Item is not exist");

        //            }

        //            if (!OutOfStock)
        //            {
        //                double ItemPrice = GetItemPrice(ItemId, CountryToUse);


        //                var CartItem = new ShoppingCart()
        //                {
        //                    CustomerId = UserId,
        //                    ItemId = ItemId,
        //                    ItemPrice = ItemPrice,
        //                    ItemQty = Quantity,
        //                    ItemTotal = Quantity * ItemPrice,

        //                };


        //                _context.ShoppingCart.Add(CartItem);
        //                _context.SaveChanges();

        //                _toastNotification.AddSuccessToastMessage("Item added to cart");
        //            }
        //            else
        //            {
        //                _toastNotification.AddWarningToastMessage("Item is out of stock");
        //            }
        //        }


        //    }
        //    catch
        //    {
        //        _toastNotification.AddErrorToastMessage("Something went wrong");

        //    }

        //    return new JsonResult(Islogin);
        //}

        public async Task<IActionResult> OnPostDeleteProducts()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var Product = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                                   .Include(e => e.Product).Include(e=>e.ShopingCartProductExtraFeatures).Include(e => e.ProductPrice)
                                                                   .ToList();
                    _context.ShoppingCarts.RemoveRange(Product);
                    _context.SaveChanges();
                    return new JsonResult(true);
                }

            }
            catch(Exception e)
            {
             toastNotification.AddErrorToastMessage("Something went wrong");
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteoneProduct(int ProductId)
        {
            try { 
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var Product = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                               .Include(e => e.Product).Include(e => e.ShopingCartProductExtraFeatures).Include(e => e.ProductPrice)
                                                              .Where(e=>e.ProductId== ProductId).FirstOrDefault();
                _context.ShoppingCarts.Remove(Product);
                    _context.SaveChanges();
                    return new JsonResult(true);
            }

        }
            catch(Exception e)
            {
             toastNotification.AddErrorToastMessage("Something went wrong");
                return Page();
    }
            return Page();
}
    }
}
