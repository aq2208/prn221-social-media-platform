using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN221_SocialMedia.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class LoginModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        private User logedInUser;

        public LoginModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string? Email { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [StringLength(50, ErrorMessage = "Email must be less than 50 characters")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must end with @gmail.com.")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be more than 7 and less than 20 characters")]
            public string? Password { get; set; }
        }

        public void OnGet(string? email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (ModelState.IsValid)
                {
                    if (isValidUser(Input.Email, Input.Password))
                    {
                        User? user = _context.Users.Where(x => x.Email == Input.Email).FirstOrDefault();
                        HttpContext.Session.SetString("UserEmail", Input.Email);
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("Username", user.Username);
                        return RedirectToPage("/SocialMedia/Feeds");
                    }

                    ModelState.AddModelError(string.Empty, "Email or password is invalid.");
                }

                return Page();
            } catch (Exception ex)
            {
                return NotFound();
            }
        }

        private bool isValidUser(string? email, string? password)
        {
            string passwordHash = Utils.HashingWithSHA256(password);

            // check user existed
            User? user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            if (user == null || user.PasswordHash != passwordHash)
            {
                return false;
            }

            logedInUser = user;
            return true;
        }
    }
}
