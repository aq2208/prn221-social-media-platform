using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN221_SocialMedia.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class RegisterModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;
        private User newUser;

        public RegisterModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }
        public bool IsRegistrationSuccessful { get; private set; }

        public class RegisterViewModel
        {
            [Required]
            [EmailAddress]
            [StringLength(50, ErrorMessage = "Email must be less than 50 characters")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must be a valid @gmail.com address")]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be more than 7 and less than 20 characters")]
            public string? Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(20, MinimumLength = 7, ErrorMessage = "ConfirmPassword must be more then 7 and less than 20 characters")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [Required]
            [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be more than 5 and less than 20 characters")]
            public string? Username { get; set; }

            [Required]
            [Phone]
            [StringLength(11, MinimumLength = 10, ErrorMessage = "PhoneNumber must be more than 10 and less than 11 numbers")]
            public string? PhoneNumber { get; set; }
        }

        public void OnGet()
        {
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
                    // check email existed
                    User? user = _context.Users.Where(x => x.Email == Input.Email).FirstOrDefault();
                    if (user != null)
                    {
                        ModelState.AddModelError(string.Empty, "Email existed.");
                    }

                    // check username existed
                    user = _context.Users.Where(x => x.Username == Input.Username).FirstOrDefault();
                    if (user != null)
                    {
                        ModelState.AddModelError(string.Empty, "Username existed.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return Page();
                    }

                    // add new user to database
                    user = new User(Input.Username, Utils.HashingWithSHA256(Input.Password), Input.Email);
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    int lastestUserProfileId = 0;
                    UserProfile? lastestUserProfile = _context.UserProfiles.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (lastestUserProfile != null)
                    {
                        lastestUserProfileId = lastestUserProfile.Id;
                    }
                    UserProfile userProfile = new UserProfile(user.Id, Input.PhoneNumber);
                    userProfile.Id = lastestUserProfileId + 1;
                    _context.UserProfiles.Add(userProfile);
                    _context.SaveChanges();

                    // show popup notify register successful
                    IsRegistrationSuccessful = true;
                    return Page();
                }

                return Page();
            } catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
