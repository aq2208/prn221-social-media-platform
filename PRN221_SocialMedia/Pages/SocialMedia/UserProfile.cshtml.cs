using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using PRN221_SocialMedia.Models;
using System.ComponentModel.DataAnnotations;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class UserProfileModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public UserProfileModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<PostViewModel> Posts { get; set; }

        [BindProperty]
        public UserProfileViewModel Input { get; set; }
        public string ProfileImageBase64 { get; set; }
        public string CoverImageBase64 { get; set; }
        public List<Post> UserPosts { get; set; }
        public bool IsAllowToEditProfile { get; set; }
        public User? User { get; set; }
        public UserProfile? UserProfile { get; set; }

        public class UserProfileViewModel
        {
            public string Email { get; set; }

            [Required]
            [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be more than 5 and less than 20 characters")]
            public string Username { get; set; }

            [Required]
            [Phone]
            [StringLength(11, MinimumLength = 10, ErrorMessage = "PhoneNumber must be more than 10 and less than 11 numbers")]
            public string PhoneNumber { get; set; }

            public IFormFile? ProfileImageFile { get; set; } // Profile image upload
            public IFormFile? CoverImageFile { get; set; } // Cover image upload

            public string? Bio { get; set; }

            [StringLength(50, ErrorMessage = "Nickname must be less than 50 characters")]
            public string? Nickname { get; set; }
        }

        public class PostViewModel
        {
            public int PostId { get; set; }
            public string AuthorUsername { get; set; }
            public string AuthorEmail { get; set; }
            public string AuthorProfileImage { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string PostImage { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public async Task<IActionResult> OnGet(string? userProfileEmail)
        {
            // check session
            string? userId = HttpContext.Session.GetString("UserId");
            string? logedInUserEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(logedInUserEmail))
            {
                return RedirectToPage("/SocialMedia/Login");
            }

            
            if (string.IsNullOrEmpty(userProfileEmail))
            {
                userProfileEmail = logedInUserEmail;
            }

            // get data
            User? user = _context.Users.FirstOrDefault(x => x.Email == userProfileEmail);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            List<UserProfile>? userProfiles = _context.UserProfiles.Where(y => y.UserId == user.Id).ToList();
            if (userProfiles == null)
            {
                return NotFound("UserProfile not found.");
            }

            UserProfile userProfile = userProfiles.First();

            User = user;
            UserProfile = userProfile;
            Posts = new List<PostViewModel>();

            Input = new UserProfileViewModel
            {
                Email = user.Email,
                Username = user.Username,
                PhoneNumber = userProfile.PhoneNumber,
                Bio = userProfile.Bio,
                Nickname = userProfile.Nickname,
                
            };

            if (userProfile.ProfileImage != null)
            {
                ProfileImageBase64 = Convert.ToBase64String(userProfile.ProfileImage);
            }

            if (userProfile.CoverImage != null)
            {
                CoverImageBase64 = Convert.ToBase64String(userProfile.CoverImage);
            }

            UserPosts = _context.Posts.Where(p => p.UserId == user.Id).OrderByDescending(x => x.CreatedAt).ToList();
            if (UserPosts != null && UserPosts.Count > 0)
            {
                foreach (var post in UserPosts)
                {
                    PostViewModel postViewModel = new PostViewModel
                    {
                        PostId = post.Id,
                        AuthorUsername = post.User.Username,
                        AuthorEmail = post.User.Email,
                        Title = post.Title,
                        Description = post.Content,
                        CreatedAt = post.CreatedAt,
                    };

                    if (post.User.UserProfiles.First().ProfileImage != null)
                    {
                        postViewModel.AuthorProfileImage = Convert.ToBase64String(post.User.UserProfiles.First().ProfileImage);
                    }

                    if (post.PostImage != null)
                    {
                        postViewModel.PostImage = Convert.ToBase64String(post.PostImage);
                    }

                    Posts.Add(postViewModel);
                }
            }

            if (logedInUserEmail == userProfileEmail)
            {
                IsAllowToEditProfile = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ModelState.IsValid)
            {
                User? user = _context.Users.FirstOrDefault(x => x.Email == Input.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                List<UserProfile>? userProfiles = _context.UserProfiles.Where(y => y.UserId == user.Id).ToList();
                if (userProfiles == null)
                {
                    return NotFound("UserProfile not found.");
                }

                UserProfile userProfile = userProfiles.First();

                // Process the profile image
                byte[] profileImageData = null;
                if (Input.ProfileImageFile != null)
                {
                    if (!IsImageFile(Input.ProfileImageFile))
                    {
                        ModelState.AddModelError("Input.ProfileImage", "Only image files are allowed.");
                        return Page();
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await Input.ProfileImageFile.CopyToAsync(memoryStream);
                        profileImageData = memoryStream.ToArray();
                    }
                }

                // Process the cover image
                byte[] coverImageData = null;
                if (Input.CoverImageFile != null)
                {
                    if (!IsImageFile(Input.CoverImageFile))
                    {
                        ModelState.AddModelError("Input.CoverImage", "Only image files are allowed.");
                        return Page();
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await Input.CoverImageFile.CopyToAsync(memoryStream);
                        coverImageData = memoryStream.ToArray();
                    }
                }

                // check duplicate username
                User? duplicateUsername = _context.Users.Where(x => x.Username == Input.Username).FirstOrDefault();
                if (duplicateUsername != null && duplicateUsername.Id != user.Id)
                {
                    ModelState.AddModelError("Input.Username", "Username existed.");
                    return Page();
                }

                // Update the database with the new profile data
                if (user != null)
                {
                    user.Username = Input.Username;
                    _context.SaveChanges();
                }
                
                if (userProfile != null)
                {
                    userProfile.PhoneNumber = Input.PhoneNumber;
                    userProfile.Nickname = Input.Nickname;
                    userProfile.Bio = Input.Bio;
                    if (profileImageData != null)
                    {
                        userProfile.ProfileImage = profileImageData;
                    }
                    if (coverImageData != null)
                    {
                        userProfile.CoverImage = coverImageData;
                    }

                    _context.UserProfiles.Update(userProfile);
                    _context.SaveChanges();
                }
            }

            return RedirectToPage(new {logedInUserEmail = Input.Email, userProfileEmail = Input.Email });
        }

        private bool IsImageFile(IFormFile file)
        {
            // Additional check based on file extension
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extensions.Contains(extension);
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
