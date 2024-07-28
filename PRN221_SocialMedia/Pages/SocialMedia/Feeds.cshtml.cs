using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class FeedsModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public FeedsModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<PostViewModel> Posts { get; set; }

        [BindProperty]
        public AddNewPostModel Input { get; set; }

        [BindProperty]
        public int? LogedInUserId { get; set; }

        [BindProperty]
        public string? LogedInUserEmail { get; set; }

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
            public List<Comment> Comments { get; set; }

            public PostViewModel()
            {
                Comments = new List<Comment>();
            }
        }

        public class AddNewPostModel
        {
            [Required]
            public int? UserId { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Title must be more than 5 and less than 50 characters")]
            public string? Title { get; set; }

            public IFormFile? Image { get; set; }

            [Required]
            [MinLength(1)]
            public string? Description { get; set; }
        }



        public async Task<IActionResult> OnGet()
        {
            // check session
            string? userId = HttpContext.Session.GetString("UserId");
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/SocialMedia/Login");
            }

            LogedInUserId = Convert.ToInt32(userId);
            LogedInUserEmail = userEmail;

            Posts = new List<PostViewModel>();

            // get data
            List<Post> posts = _context.Posts.Include(x => x.Comments).ThenInclude(y => y.User).Include(x => x.User).ThenInclude(y => y.UserProfiles).OrderByDescending(x => x.CreatedAt).ToList();

            if (posts != null && posts.Count > 0)
            {
                foreach (var post in posts)
                {
                    PostViewModel postViewModel = new PostViewModel
                    {
                        PostId = post.Id,
                        AuthorUsername = post.User.Username,
                        AuthorEmail = post.User.Email,
                        Title = post.Title,
                        Description = post.Content,
                        CreatedAt = post.CreatedAt,
                        Comments = new List<Comment>(),
                    };

                    if (post.Comments != null)
                    {
                        foreach (Comment comment in post.Comments)
                        {
                            postViewModel.Comments.Add(comment);
                        }
                    }

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

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _context.Users.Where(x => x.Id == Input.UserId).First();
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Process the profile image
            byte[] ImageData = null;
            if (Input.Image != null)
            {
                if (!IsImageFile(Input.Image))
                {
                    ModelState.AddModelError("Input.Image", "Only image files are allowed.");
                    return Page();
                }

                using (var memoryStream = new MemoryStream())
                {
                    await Input.Image.CopyToAsync(memoryStream);
                    ImageData = memoryStream.ToArray();
                }
            }

            // create new post
            var newPost = new Post
            {
                UserId = user.Id,
                Title = Input.Title,
                Content = Input.Description,
            };

            if (ImageData != null)
            {
                newPost.PostImage = ImageData;
            }

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return RedirectToPage(new { logedInUserEmail = user.Email});
        }

        private bool IsImageFile(IFormFile file)
        {
            // Additional check based on file extension
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extensions.Contains(extension);
        }
    }
}
