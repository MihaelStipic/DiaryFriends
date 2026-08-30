using System.Collections.Generic;

namespace DiaryFriends.Models
{
    public class DiaryIndexViewModel
    {
        public List<DiaryEntry> Entries { get; set; }
        public string FirstName { get; set; }
        public int StreakCount { get; set; }
    }
}