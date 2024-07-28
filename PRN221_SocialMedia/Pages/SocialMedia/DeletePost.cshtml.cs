using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PRN221_SocialMedia.Models;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class DeletePostModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public DeletePostModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(string? postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return NotFound("postId not found.");
            }

            Post? deletePost = _context.Posts.Include(x => x.User).Where(x => x.Id == Convert.ToInt32(postId)).FirstOrDefault();
            if (deletePost == null)
            {
                return NotFound("postId not found.");
            }

            string userEmail = deletePost.User.Email;
            _context.Posts.Remove(deletePost);
            _context.SaveChanges();

            return RedirectToPage("/SocialMedia/UserProfile", new { logedInUserEmail = deletePost.User.Email, userProfileEmail = deletePost.User.Email });
        }
    }
}
