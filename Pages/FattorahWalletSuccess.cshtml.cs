using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Vision.Data;
using Vision.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Vision.Pages
{
    public class FattorahWalletSuccessModel : PageModel
    {
        private readonly CRMDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationDbContext _applicationDbContext { get; set; }

        public WalletSubscription walletSubscription { set; get; }
        
        public ApplicationUser user { set; get; }
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private readonly IConfiguration _configuration;
        FattorhResult FattoraResStatus { set; get; }
        public static bool expired = false;
        string res { set; get; }
        public FattorahWalletSuccessModel(CRMDBContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager,  IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _context = context;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }
        public FattorahPaymentResult fattorahPaymentResult { get; set; }
        static string token = "rLtt6JWvbUHDDhsZnfpAhpYk4dxYDQkbcPTyGaKp2TYqQgG7FGZ5Th_WD53Oq8Ebz6A53njUoo1w3pjU1D4vs_ZMqFiz_j0urb_BH9Oq9VZoKFoJEDAbRZepGcQanImyYrry7Kt6MnMdgfG5jn4HngWoRdKduNNyP4kzcp3mRv7x00ahkm9LAK7ZRieg7k1PDAnBIOG3EyVSJ5kK4WLMvYr7sCwHbHcu4A5WwelxYK0GMJy37bNAarSJDFQsJ2ZvJjvMDmfWwDVFEVe_5tOomfVNt6bOg9mexbGjMrnHBnKnZR1vQbBtQieDlQepzTZMuQrSuKn-t5XZM7V6fCW7oP-uXGX-sMOajeX65JOf6XVpk29DP6ro8WTAflCDANC193yof8-f5_EYY-3hXhJj7RBXmizDpneEQDSaSz5sFk0sV5qPcARJ9zGG73vuGFyenjPPmtDtXtpx35A-BVcOSBYVIWe9kndG3nclfefjKEuZ3m4jL9Gg1h2JBvmXSMYiZtp9MR5I6pvbvylU_PP5xJFSjVTIz7IQSjcVGO41npnwIxRXNRxFOdIUHn0tjQ-7LwvEcTXyPsHXcMD8WtgBh-wxR8aKX7WPSsT1O8d8reb2aR7K3rkV3K82K_0OgawImEpwSvp9MNKynEAJQS6ZHe_J_l77652xwPNxMRTMASk1ZsJL";
        static string testURL = "https://apitest.myfatoorah.com/v2/GetPaymentStatus";
        static string liveURL = "https://api.myfatoorah.com/v2/GetPaymentStatus";
        public async Task<IActionResult> OnGet(string paymentId)
        {
            if (paymentId != null)
            {
                var GetPaymentStatusRequest = new
                {
                    Key = paymentId,
                    KeyType = "paymentId"
                };
                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];

                var GetPaymentStatusRequestJSON = JsonConvert.SerializeObject(GetPaymentStatusRequest);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Fattorahstatus) // fattorah live
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(GetPaymentStatusRequestJSON, System.Text.Encoding.UTF8, "application/json");
                    var responseMessage = client.PostAsync(liveURL, httpContent);
                    res = await responseMessage.Result.Content.ReadAsStringAsync();
                    FattoraResStatus = JsonConvert.DeserializeObject<FattorhResult>(res);
                }
                else                 // fattorah test
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(GetPaymentStatusRequestJSON, System.Text.Encoding.UTF8, "application/json");
                    var responseMessage = client.PostAsync(testURL, httpContent);
                    res = await responseMessage.Result.Content.ReadAsStringAsync();
                    FattoraResStatus = JsonConvert.DeserializeObject<FattorhResult>(res);
                }

                if (FattoraResStatus.IsSuccess == true)
                {
                    Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                    fattorahPaymentResult = jObject["Data"].ToObject<FattorahPaymentResult>();
                    int WalletSubId = 0;
                    bool checkRes = int.TryParse(fattorahPaymentResult.UserDefinedField, out WalletSubId);
                    if (fattorahPaymentResult.InvoiceStatus == "Paid")
                    {
                        try
                        {
                            if (fattorahPaymentResult.UserDefinedField != null)
                            {

                                if (checkRes)
                                {
                                    
                                        walletSubscription = _context.WalletSubscriptions.Where(e => e.WalletSubscriptionId == WalletSubId).FirstOrDefault();
                                        var WalletObj = _context.Wallets.Where(e => e.WalletId == walletSubscription.WalletId).FirstOrDefault();
                                        user = await _userManager.FindByIdAsync(walletSubscription.UserId);
                                        walletSubscription.IsPaid = true;
                                        walletSubscription.PaymentID = paymentId;
                                        
                                        var UpdatedwalletSubscription = _context.WalletSubscriptions.Attach(walletSubscription);
                                        UpdatedwalletSubscription.State = EntityState.Modified;
                                        user.AvailableClassified = user.AvailableClassified + WalletObj.NumberOfClassifed;
                                        _applicationDbContext.SaveChanges();
                                        _context.SaveChanges();
                                       
                                    return Page();

                                }
                                return RedirectToPage("SomethingwentError");

                            }
                        }
                        catch (Exception)
                        {
                            return RedirectToPage("SomethingwentError");
                        }


                    }
                    else
                    {
                        try
                        {
                            if (fattorahPaymentResult.UserDefinedField != null)
                            {
                                if (checkRes)
                                {
                                   

                                        walletSubscription = _context.WalletSubscriptions.Where(e => e.WalletSubscriptionId == WalletSubId).FirstOrDefault();



                                        _context.WalletSubscriptions.Remove(walletSubscription);
                                        _context.SaveChanges();
                                    
                                    return Page();
                                }
                                return RedirectToPage("SomethingwentError");
                            }
                        }

                        catch (Exception)
                        {
                            return RedirectToPage("SomethingwentError");
                        }
                    }

                }

            }

            return RedirectToPage("SomethingwentError");
        }

    }
}
