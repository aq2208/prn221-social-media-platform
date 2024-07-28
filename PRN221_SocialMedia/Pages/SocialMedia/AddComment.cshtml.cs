using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Models;
using System.ComponentModel.Design;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class AddCommentModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public AddCommentModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int PostId { get; set; }


        [BindProperty]
        public string CommentText { get; set; }

        public async Task<IActionResult> OnGet(int? postId, string? content)
        {
            try
            {
                if (postId == null || string.IsNullOrEmpty(content))
                {
                    return NotFound("PostId or Comment content not found.");
                }

                // check session
                int? userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                if (userId == null)
                {
                    return RedirectToPage("/SocialMedia/Login");
                }

                //
                var post = _context.Posts.Where(x => x.Id == postId).FirstOrDefault();
                if (post == null)
                {
                    return new JsonResult(new { success = false });
                }

                Comment comment = new Comment
                {
                    PostId = (int)postId,
                    Content = content,
                    UserId = (int)userId,
                };

                _context.Comments.Add(comment);
                _context.SaveChanges();

                //return new JsonResult(new { success = true });
                return new JsonResult(new { success = true, commentId = comment.Id, commentText = CommentText, authorUsername = HttpContext.Session.GetString("Username") });
            } catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post == null)
            {
                return new JsonResult(new { success = false });
            }

            var comment = new Comment
            {
                PostId = PostId,
                Content = CommentText,
                UserId = 1,
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
