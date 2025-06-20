using System;
using System.Linq; // Added for .FirstOrDefault()
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics; // Added for Debug.WriteLine

namespace JobJournal.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                StatusMessage = $"Error: Unable to load user with ID '{userId}'.";
                Debug.WriteLine($"ConfirmEmailChange: User not found for ID '{userId}'"); // Debug output
                return Page();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ChangeEmailAsync(user, email, code);

            if (!result.Succeeded)
            {
                // *** IMPORTANT CHANGE HERE: Log and display the FIRST error description ***
                var errorDescription = result.Errors.FirstOrDefault()?.Description ?? "Unknown error.";
                StatusMessage = $"Error changing email: {errorDescription}";
                Debug.WriteLine($"ConfirmEmailChange: Email change failed for user '{userId}'. Error: {errorDescription}"); // Debug output
                return Page();
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                // If this fails, it's a secondary issue but still needs to be noted.
                var setUserNameError = setUserNameResult.Errors.FirstOrDefault()?.Description ?? "Unknown user name error.";
                StatusMessage = $"Email changed, but error updating username: {setUserNameError}";
                Debug.WriteLine($"ConfirmEmailChange: Username update failed for user '{userId}'. Error: {setUserNameError}"); // Debug output
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Thank you for confirming your email change. Your email has been updated.";
            Debug.WriteLine($"ConfirmEmailChange: Email successfully changed for user '{userId}' to '{email}'."); // Debug output
            return Page();
        }
    }
}
