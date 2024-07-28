using System;
using System.Collections.Generic;

namespace PRN221_SocialMedia.Models
{
    public partial class UserProfile
    {
        public UserProfile() { }

        public UserProfile(int userId, string phoneNumber) 
        {
            this.UserId = userId;
            this.PhoneNumber = phoneNumber;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? CoverPicture { get; set; }
        public string? Nickname { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public byte[]? ProfileImage { get; set; }
        public byte[]? CoverImage { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
