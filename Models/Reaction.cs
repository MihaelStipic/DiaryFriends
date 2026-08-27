using System.ComponentModel.DataAnnotations;

namespace DiaryFriends.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DiaryEntryId { get; set; }

        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? ReactionType { get; set; }
    }
}