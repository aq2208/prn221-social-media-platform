using System;
using System.Collections.Generic;

namespace PRN221_SocialMedia.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Media { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[]? PostImage { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
