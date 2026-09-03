using System.Collections.Generic;

namespace DiaryFriends.Models
{
    public class MyPageViewModel
    {
        public string? CurrentUserId { get; set; }
        public string? FirstName { get; set; }
        public ApplicationUser? Friend { get; set; }
        public TicTacToe? Game { get; set; }
        public string[] Board { get; set; } = new string[9];
        public List<DiaryEntry> Entries { get; set; } = new List<DiaryEntry>();
        public string? ProfilePicturePath { get; set; }
    }
}