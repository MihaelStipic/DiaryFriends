using System.ComponentModel.DataAnnotations;

namespace DiaryFriends.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string FriendId { get; set; } = string.Empty;
    }
}