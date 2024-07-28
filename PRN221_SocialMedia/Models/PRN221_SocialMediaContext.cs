using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRN221_SocialMedia.Models
{
    public partial class PRN221_SocialMediaContext : DbContext
    {
        public PRN221_SocialMediaContext()
        {
        }

        public PRN221_SocialMediaContext(DbContextOptions<PRN221_SocialMediaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Like> Likes { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserProfile> UserProfiles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectStr = config.GetConnectionString("DemoConnectStr");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.ChatMessageReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatMessa__Recei__5FB337D6");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.ChatMessageSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__ChatMessa__Sende__5EBF139D");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__Comments__PostId__5535A963");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comments__UserId__5629CD9C");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__Likes__PostId__59FA5E80");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Likes__UserId__5AEE82B9");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Media).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Posts__UserId__440B1D61");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4B44380D4")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534EFA01BA5")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.PasswordHash).HasMaxLength(256);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Bio).HasMaxLength(500);

                entity.Property(e => e.CoverPicture).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Nickname).HasMaxLength(500);

                entity.Property(e => e.ProfilePicture).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProfiles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserProfi__UserI__3F466844");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
