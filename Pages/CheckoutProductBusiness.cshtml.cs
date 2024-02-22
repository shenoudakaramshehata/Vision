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
    public class CheckoutProductBusinessModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToastNotification _toastNotification;
        public HttpClient httpClient { get; set; }
       public CustomerAddress customerAddress { get; set; }
        public List<ShoppingCart> cartProducts { get; set; }
        public ShoppingCart cartShippingCost { get; set; }
        public double totalCost { get; set; }
        public double cartSubTotal { get; set; }
        public ShoppingCart businessCheckout { get; set; }
        public ApplicationUser user { get; set; }
        [BindProperty]
        public bool isFastOrder { get; set; } = true;
        [BindProperty]
        public string fastOrder { get; set; }
        [BindProperty]
        public CheckOutVM checkOutVM { get; set; }
        public CheckoutProductBusinessModel(CRMDBContext Context, IToastNotification toastNotification, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = Context;
            _userManager = userManager;
            _toastNotification = toastNotification;
            httpClient = new HttpClient();
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }

                customerAddress = _context.CustomerAddresses.Where(i => i.UserId == user.Id).FirstOrDefault();
                if (customerAddress == null)
                {
                    customerAddress = new CustomerAddress();
                }

                cartProducts = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                    .ToList();

                businessCheckout = _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                    .Include(e => e.Product)
                    .ThenInclude(e => e.ProductCategory)
                    .ThenInclude(e => e.ClassifiedBusiness)
                    .FirstOrDefault();
                foreach (var product in cartProducts)
                {
                    cartSubTotal += product.ProductTotal;
                }

                cartShippingCost = await _context.ShoppingCarts.Where(e => e.UserId == user.Id)
                                                                          .Include(e => e.Product)
                                                                          .ThenInclude(s => s.ProductCategory)
                                                                          .ThenInclude(s => s.ClassifiedBusiness)
                                                                          .FirstOrDefaultAsync();
                if (cartShippingCost != null)
                {
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

        public async Task<IActionResult> OnPostAddOrder(CheckOutVM checkOutVM)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Redirect("/identity/account/login");
                }
                
                if (checkOutVM.FastOrder == false)
                {
                    var customerAddressObj = _context.CustomerAddresses.FirstOrDefault(c => c.CustomerAddressId == checkOutVM.CustomerAddressId);
                    if (customerAddressObj == null)
                    {
                        _toastNotification.AddErrorToastMessage("Customer Address Object Not Found");
                        return Page();

                    }
                }


                double discount = 0;

                var customerShoppingCartList = _context.ShoppingCarts
                    .Include(e => e.ShopingCartProductExtraFeatures)
                    .Include(e => e.ProductPrice)
                    .Include(s => s.Product)
                    .ThenInclude(s => s.ProductCategory).ThenInclude(s => s.ClassifiedBusiness)
                .Where(c => c.UserId == user.Id).ToList();

                var customerShoppingCartExtraList = _context.ShopingCartProductExtraFeatures.Where(e => e.ShoppingCartId == customerShoppingCartList.FirstOrDefault().ShoppingCartId).ToList();

                var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ProductTotal);


                int maxUniqe = 1;
                var newList = _context.Orders.ToList();
                if (newList.Count > 0)
                {
                    maxUniqe = newList.Max(e => e.UniqeId);
                }
                var orders = customerShoppingCartList.AsEnumerable().GroupBy(c => c.Product.ProductCategory.ClassifiedBusinessId).

                Select(g => new Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Guid.NewGuid().ToString() + "/" + DateTime.Now.Year,
                    ClassifiedBusinessId = g.Key,
                    UserId = user.Id,
                    CustomerAddressId = checkOutVM.FastOrder == false ? checkOutVM.CustomerAddressId : null,
                    Adress = checkOutVM.FastOrder == true ? checkOutVM.Address : null,
                    IsPaid = false,
                    OrderTotal = g.Sum(c => c.ProductTotal),
                    Deliverycost = _context.ClassifiedBusiness.FirstOrDefault(s => s.ClassifiedBusinessId == g.Key).Deliverycost,
                    OrderNet = g.Sum(c => c.ProductTotal) + _context.ClassifiedBusiness.FirstOrDefault(s => s.ClassifiedBusinessId == g.Key).Deliverycost,
                    Discount = discount,
                    UniqeId = maxUniqe + 1
                }).ToList();


                foreach (var item in orders)
                {
                    _context.Orders.Add(item);
                    _context.SaveChanges();

                    var orderitemextrafeatures = new List<OrderItemExtraProduct>();

                    foreach (var itemshop in customerShoppingCartList)
                    {

                        if (itemshop.Product.ProductCategory.ClassifiedBusinessId == item.ClassifiedBusinessId)
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                ProductId = (int)itemshop.ProductId,
                                ItemPrice = itemshop.ItemPrice,
                                ProductPriceId = itemshop.ProductPriceId,
                                Total = itemshop.ProductTotal,
                                ProductQuantity = itemshop.ProductQty,
                                OrderId = item.OrderId
                            };
                            _context.OrderItems.Add(orderItem);
                            _context.SaveChanges();
                            if (customerShoppingCartExtraList != null)
                            {
                                if (customerShoppingCartExtraList.Count != 0)
                                {
                                    foreach (var itemsExtra in customerShoppingCartExtraList)
                                    {

                                        OrderItemExtraProduct OrderItemExtraProductobj = new OrderItemExtraProduct
                                        {
                                            OrderItemId = orderItem.OrderItemId,

                                            ProductExtraId = itemsExtra.ProductExtraId,
                                            Price = itemsExtra.Price,

                                        };

                                        orderitemextrafeatures.Add(OrderItemExtraProductobj);

                                        orderItem.OrderItemExtraProducts = orderitemextrafeatures;
                                        _context.OrderItemExtraProducts.Add(OrderItemExtraProductobj);
                                    }


                                    _context.SaveChanges();

                                }
                            }

                        }
                    }
                }
                _context.RemoveRange(customerShoppingCartExtraList);
                _context.RemoveRange(customerShoppingCartList);

                _toastNotification.AddSuccessToastMessage("Order processed successfully");

            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage(ex.Message);
            }
            return Page();
        }
    }
}
