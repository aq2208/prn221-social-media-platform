using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class LogoutModel : PageModel
    {

        public LogoutModel()
        {
        }

        public async Task<IActionResult> OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/SocialMedia/Login");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/SocialMedia/Login");
        }
    }
}
