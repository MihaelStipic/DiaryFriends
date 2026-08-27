using Microsoft.AspNetCore.Identity;

namespace DiaryFriends.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public int StreakCount { get; set; }
        public DateTime StreakDate { get; set; }
    }
}