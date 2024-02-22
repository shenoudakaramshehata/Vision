using Vision.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using Vision.ViewModel;

namespace Vision
{
    public class ChangePasswordPModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public ChangePasswordVM changePasswordVM { get; set; }
        public ChangePasswordPModel(UserManager<ApplicationUser> userManager, IToastNotification toastNotification, SignInManager<ApplicationUser> signInManager)
        {
            _userManger = userManager;
            _toastNotification = toastNotification;
            _signInManager = signInManager;
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost()
        {
            if (changePasswordVM.CurrentPassword==changePasswordVM.NewPassword)
            { 
                ModelState.AddModelError("","New password Must be Diffrent from Current Password..");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();
            var user = await _userManger.GetUserAsync(User);
            var result = await _userManger.ChangePasswordAsync(user, changePasswordVM.CurrentPassword, changePasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            await _signInManager.RefreshSignInAsync(user);
            _toastNotification.AddSuccessToastMessage("Password Updated Successfully");
            return RedirectToPage("/Profile");

        }
    }
}

