using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Models;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class AddMessageModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public AddMessageModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet(string senderId, string receiverId, string message)
        {
            if (ModelState.IsValid)
            {
                var newMessage = new ChatMessage
                {
                    SenderId = Convert.ToInt32(senderId),
                    ReceiverId = Convert.ToInt32(receiverId),
                    Content = message
                };

                _context.ChatMessages.Add(newMessage);
                _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }

        public void OnPost() 
        {

        }
    }
}
