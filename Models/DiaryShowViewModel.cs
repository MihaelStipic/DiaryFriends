using System.Collections.Generic;

namespace DiaryFriends.Models
{
    public class DiaryShowViewModel
    {
        public List<DiaryEntry> Entries { get; set; }
        public string FriendName { get; set; }
        public int StreakCount { get; set; }
        public DateTime? StreakDate { get; set; }
    }
}