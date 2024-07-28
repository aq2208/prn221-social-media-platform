using System;
using System.Collections.Generic;

namespace PRN221_SocialMedia.Models
{
    public partial class User
    {
        public User()
        {
            ChatMessageReceivers = new HashSet<ChatMessage>();
            ChatMessageSenders = new HashSet<ChatMessage>();
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
            Posts = new HashSet<Post>();
            UserProfiles = new HashSet<UserProfile>();
        }

        public User(string username, string passwordHash, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<ChatMessage> ChatMessageReceivers { get; set; }
        public virtual ICollection<ChatMessage> ChatMessageSenders { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
    }
}
