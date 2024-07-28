using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Models;
using System.ComponentModel.DataAnnotations;
using static PRN221_SocialMedia.Pages.SocialMedia.FeedsModel;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class UpdatePostModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public UpdatePostModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int PostId { get; set; }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public IFormFile PostImage { get; set; }

        public string PostImageUrl { get; set; }

        [BindProperty]
        public Post selectedPost { get; set; }

        public async Task<IActionResult> OnGet(int? postId, string? userEmail)
        {
            if (postId == null) return NotFound("PostId not found.");

            var post = _context.Posts.Where(x => x.Id == postId).FirstOrDefault();
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            PostId = post.Id;
            Title = post.Title;
            Description = post.Content;

            if (post.PostImage != null)
            {
                PostImageUrl = $"data:image/png;base64,{Convert.ToBase64String(post.PostImage)}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var post = _context.Posts.Include(x => x.User).Where(x => x.Id == PostId).FirstOrDefault();
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            post.Title = Title;
            post.Content = Description;

            if (PostImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await PostImage.CopyToAsync(memoryStream);
                    post.PostImage = memoryStream.ToArray();
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/SocialMedia/UserProfile", new { logedInUserEmail = post.User.Email, userProfileEmail = post.User.Email});
        }
    }
}
