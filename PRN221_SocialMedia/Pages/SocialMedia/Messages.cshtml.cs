using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Models;

namespace PRN221_SocialMedia.Pages.SocialMedia
{
    public class MessagesModel : PageModel
    {
        private readonly PRN221_SocialMediaContext _context;

        public MessagesModel(PRN221_SocialMediaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int? CurrentChatUserId { get; set; }

        [BindProperty]
        public int? LoggedInUserId { get; set; }

        [BindProperty]
        public List<User> Users { get; set; }

        [BindProperty]
        public List<ChatMessage> MessagesWithUser { get; set; }

        public async Task<IActionResult> OnGet(int? userId)
        {
            // check session
            int? logedInUserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            if (!logedInUserId.HasValue || logedInUserId == null)
            {
                return RedirectToPage("/SocialMedia/Login");
            }

            LoggedInUserId = logedInUserId;

            // Get the users the logged-in user has messaged with
            var sentMessages = _context.ChatMessages
                            .Where(m => m.SenderId == logedInUserId).OrderByDescending(x => x.SentAt)
                            .Select(m => m.ReceiverId);

            var receivedMessages = _context.ChatMessages
                            .Where(m => m.ReceiverId == logedInUserId).OrderByDescending(x => x.SentAt)
                            .Select(m => m.SenderId);

            var userIds = sentMessages.Union(receivedMessages).Distinct();

            Users = await _context.Users.Include(x => x.UserProfiles)
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

            // get all the messages with a specific userId
            if (userId.HasValue)
            {
                if (!Users.Select(x => x.Id).Contains(userId.Value))
                {
                    User? startChatUser = _context.Users.Include(x => x.UserProfiles).Where(x => x.Id == userId).FirstOrDefault();
                    if (startChatUser != null)
                    {
                        Users.Insert(0, startChatUser);
                        //Users.Add(startChatUser);
                    }
                }

                CurrentChatUserId = userId;
                MessagesWithUser = _context.ChatMessages
                    .Include(m => m.Sender)
                    .Where(m => (m.SenderId == LoggedInUserId && m.ReceiverId == userId)
                             || (m.SenderId == userId && m.ReceiverId == LoggedInUserId))
                    .OrderBy(m => m.SentAt)
                    .ToList();
            }
            else
            {
                MessagesWithUser = new List<ChatMessage>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(string senderId, string receiverId, string message)
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSaveMessageAsync(string senderId, string receiverId, string message)
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
    }
}
