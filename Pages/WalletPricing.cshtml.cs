using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vision.Data;
using Vision.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;



namespace Vision.Pages
{
    public class WalletPricingModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<Wallet> Wallets = new List<Wallet>();
        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        public WalletPricingModel(CRMDBContext Context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _context = Context;
            _userManager = userManager;
            _configuration = configuration;
            httpClient = new HttpClient();
        }
        public async Task<ActionResult> OnGet()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Redirect("/identity/account/login");

            //}
            Wallets = _context.Wallets.ToList();
            return Page();
        }
        public async Task<ActionResult> OnPost(int walletId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/identity/account/login");

            }
            var wallet = _context.Wallets.Find(walletId);
            if (wallet == null)
            {
                return Redirect("/PageNF");
            }


            var walletSubscription = new WalletSubscription()
            {
                IsPaid = false,
                PaymentMethodId = 1,
                SubscriptionDate = DateTime.Now,
                UserId = user.Id,
                WalletId = walletId,
                PaymentID = " ",

            };
            _context.WalletSubscriptions.Add(walletSubscription);
            _context.SaveChanges();
            int PaymentMethodId = 1;
            if (PaymentMethodId == 1)
            {
                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = wallet.Price,
                        CallBackUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahWalletSuccess",
                        ErrorUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahWalletError",

                        UserDefinedField = walletSubscription.WalletSubscriptionId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://api.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Redirect(InvoiceRes.InvoiceURL);



                    }
                    else
                    {

                        _context.WalletSubscriptions.Remove(walletSubscription);
                        _context.SaveChanges();
                        return RedirectToPage("SomethingwentError");


                    }
                }
                else               //fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = user.FullName,
                        NotificationOption = "LNK",
                        InvoiceValue = wallet.Price,
                        CallBackUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahWalletSuccess",
                        ErrorUrl = "http://codewarenet-001-site21.dtempurl.com/FattorahWalletError",

                        UserDefinedField = walletSubscription.WalletSubscriptionId,
                        CustomerEmail = user.Email
                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Redirect(InvoiceRes.InvoiceURL);



                    }
                    else
                    {

                        _context.WalletSubscriptions.Remove(walletSubscription);
                        _context.SaveChanges();
                        return RedirectToPage("SomethingwentError");


                    }
                }




            }
            return Page();
        }
    }
}
