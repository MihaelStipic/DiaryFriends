using System.ComponentModel.DataAnnotations;

namespace DiaryFriends.Models
{
    public class TicTacToe
    {
        [Key]
        public int Id { get; set; }
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public string CurrentTurnPlayerId { get; set; }
        public string BoardState { get; set; } = "-,-,-,-,-,-,-,-,-";
        public string? WinnerId { get; set; }
        public bool IsGameOver { get; set; } = false;
    }
}