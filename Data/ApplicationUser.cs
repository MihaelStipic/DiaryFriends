using Microsoft.AspNetCore.Identity;

namespace DiaryFriends.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
    }
}