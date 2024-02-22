using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.Data;
using Vision.Models;

namespace Vision.Areas.CRM.Pages.Configurations.ManageWallet
{
    public class IndexModel : PageModel
    {
		private CRMDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IToastNotification _toastNotification;
		public string url { get; set; }
		public List<Wallet> wallets { get; set; }
		[BindProperty]
		public Wallet Wallet { get; set; }

		[BindProperty(SupportsGet = true)]
		public bool ArLang { get; set; }

		public IndexModel(CRMDBContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
			_toastNotification = toastNotification;
            Wallet = new Wallet();
			wallets = new List<Wallet>();
        }
        public void OnGet()
		{
            wallets = _context.Wallets.OrderByDescending(e=>e.SortOrder).ToList();
			url = $"{this.Request.Scheme}://{this.Request.Host}";
			var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
			var BrowserCulture = locale.RequestCulture.UICulture.ToString();
			if (BrowserCulture == "en-US")

				ArLang = false;

			else
				ArLang = true;
		}


        public IActionResult OnGetSingleWalletForEdit(int WalletId)
        {
            var Result = _context.Wallets.Where(c => c.WalletId == WalletId).FirstOrDefault();
            return new JsonResult(Result);

        }


        public async Task<IActionResult> OnPostEditforWallet(int WalletId)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();

            if (!ModelState.IsValid)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("Please Enter All the Required Data");

                else
                    _toastNotification.AddErrorToastMessage("من فضلك ادخل البيانات كاملة");
                return Redirect("/CRM/Configurations/ManageWallet/Index");
            }
            try
            {
                var model = _context.Wallets.Where(c => c.WalletId == WalletId).FirstOrDefault();
                if (model == null)
                {
                    if (BrowserCulture == "en-US")

                        _toastNotification.AddErrorToastMessage("Wallet is not Existed");

                    else
                        _toastNotification.AddErrorToastMessage("المحفظة غير موجودة");

                    return Redirect("/CRM/Configurations/ManageWallet/Index");
                }

              
                model.WalletTitleAr = Wallet.WalletTitleAr;
                model.WalletTitleEn = Wallet.WalletTitleEn;
                model.Price = Wallet.Price;
                model.NumberOfClassifed = Wallet.NumberOfClassifed;
                model.SortOrder = Wallet.SortOrder;

                model.IsActive = Wallet.IsActive;


                var Updatedwallet = _context.Wallets.Attach(model);
                Updatedwallet.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                await _context.SaveChangesAsync();
                if (BrowserCulture == "en-US")

                    _toastNotification.AddSuccessToastMessage("Wallet Edited successfully ");

                else
                    _toastNotification.AddSuccessToastMessage("تم تعديل المحفظة بنجاح");


            }
            catch (Exception)
            {
                if (BrowserCulture == "en-US")

                    _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");

                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");

            }
            return Redirect("/CRM/Configurations/ManageWallet/Index");
        }


        public IActionResult OnGetSingleWalletForView(int WalletId)
        {
            var Result = _context.Wallets.Where(c => c.WalletId == WalletId).Select(i => new
            {
                WalletId = i.WalletId,
                WalletTitleAr = i.WalletTitleAr,
                WalletTitleEn = i.WalletTitleEn,
                NumberOfClassifed = i.NumberOfClassifed,
                Price = i.Price,
                SortOrder = i.SortOrder,
                IsActive = i.IsActive,

            }).FirstOrDefault();

            return new JsonResult(Result);
        }


        public IActionResult OnGetSingleWalletForDelete(int WalletId)
        {
            Wallet = _context.Wallets.Where(c => c.WalletId == WalletId).FirstOrDefault();
            return new JsonResult(Wallet);
        }


        public async Task<IActionResult> OnPostDeleteWallets(int WalletId)
        {
            var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var BrowserCulture = locale.RequestCulture.UICulture.ToString();
            try
            {
                Wallet walletObj = _context.Wallets.Where(e => e.WalletId == WalletId).FirstOrDefault();


                if (walletObj != null)
                {
                   
                    _context.Wallets.Remove(walletObj);
                    await _context.SaveChangesAsync();
                    if (BrowserCulture == "en-US")

                        _toastNotification.AddSuccessToastMessage("Wallet Deleted successfully");
                     else
                        _toastNotification.AddSuccessToastMessage("تم مسح المحفظة بنجاح");

                }
                else
                {
                    if (BrowserCulture == "en-US")
                        _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
                    else
                        _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
                }
            }
            catch (Exception)

            {
                if (BrowserCulture == "en-US")
                    _toastNotification.AddErrorToastMessage("SomeThing Went Wrong");
                else
                    _toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
            }

            return Redirect("/CRM/Configurations/ManageWallet/Index");
        }


        public IActionResult OnPostAddWallet()
		{
			var locale = Request.HttpContext.Features.Get<IRequestCultureFeature>();
			var BrowserCulture = locale.RequestCulture.UICulture.ToString();

			if (!ModelState.IsValid)
			{
				if (BrowserCulture == "en-US")

					_toastNotification.AddErrorToastMessage("Please Enter All the Required Data");

				else
					_toastNotification.AddErrorToastMessage("من فضلك ادخل البيانات كاملة");
				return Redirect("/CRM/Configurations/ManageWallet/Index");
			}
			try
			{
			
				_context.Wallets.Add(Wallet);
				_context.SaveChanges();
				if (BrowserCulture == "en-US")

					_toastNotification.AddSuccessToastMessage("Wallet added successfully");

				else
					_toastNotification.AddSuccessToastMessage("تم اضافة المحفظة");

			}
			catch (Exception)
			{
				if (BrowserCulture == "en-US")

					_toastNotification.AddErrorToastMessage("SomeThing Went Wrong");

				else
					_toastNotification.AddErrorToastMessage("حدث شئ خاطئ");
			}
			return Redirect("/CRM/Configurations/ManageWallet/Index");
		}


		

	}
}
