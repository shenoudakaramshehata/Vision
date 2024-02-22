using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Vision.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Vision.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Vision.Pages
{
    public class FattorahOrderSuccessModel : PageModel
    {
        private readonly CRMDBContext _context;
        public List<Order> order { get; set; }
        private readonly IConfiguration _configuration;
        FattorhResult FattoraResStatus { set; get; }
        string res { set; get; }
        public FattorahOrderSuccessModel(CRMDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public FattorahPaymentResult fattorahPaymentResult { get; set; }
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
                    int orderId = 0;
                    bool checkRes = int.TryParse(fattorahPaymentResult.UserDefinedField, out orderId);
                    if (fattorahPaymentResult.InvoiceStatus == "Paid")
                    {
                        try
                        {
                            if (fattorahPaymentResult.UserDefinedField != null)
                            {

                                if (checkRes)
                                {

                                    order = await _context.Orders.Where(e => e.UniqeId == orderId).ToListAsync();

                                    foreach (var item in order)
                                    {
                                        item.IsPaid = true;
                                        var UpdatedOrder = _context.Orders.Attach(item);
                                        UpdatedOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                                        _context.SaveChanges();
                                    }


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

                                    order = _context.Orders.Where(e => e.UniqeId == orderId).ToList();
                                    foreach (var item in order)
                                    {
                                        var orderItems = _context.OrderItems.Where(e => e.OrderId == item.OrderId).ToList();
                                        if (orderItems != null)
                                        {
                                            _context.OrderItems.RemoveRange(orderItems);
                                        }
                                    }
                                    _context.Orders.RemoveRange(order);
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
