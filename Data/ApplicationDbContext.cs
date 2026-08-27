using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DiaryFriends.Models;
using Microsoft.EntityFrameworkCore;

namespace DiaryFriends.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<DiaryEntry> DiaryEntries { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var defaultDate = new DateTime(2026, 1, 1, 0, 0, 0);
            modelBuilder.Entity<DiaryEntry>().HasData(
                new DiaryEntry { Id = 1, Title = "Went Hiking", Content = "Hiking with me", Created = defaultDate, UserId = "5d0370d0-678e-4144-b645-3303335f8d75"},
                new DiaryEntry { Id = 2, Title = "Went Fishin", Content = "Fishin with me", Created = defaultDate, UserId = "5d0370d0-678e-4144-b645-3303335f8d75" },
                new DiaryEntry { Id = 3, Title = "Went Shopping", Content = "Shop with me", Created = defaultDate, UserId = "5d0370d0-678e-4144-b645-3303335f8d75"}
                );
        }
    }
}

