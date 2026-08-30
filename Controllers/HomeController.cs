
using DiaryFriends.Data;
using DiaryFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace DiaryFriends.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
        [Authorize]
        public async Task<IActionResult> MyPage()
        {
            var currentUserId = GetCurrentUserId();

            var currentUser = await _db.Users.FindAsync(currentUserId);
            var firstName = currentUser?.FirstName ?? User.Identity?.Name;

            var f = await _db.Friends.FirstOrDefaultAsync(x => x.UserId == currentUserId || x.FriendId == currentUserId);
            var friendId = f == null ? null : (f.UserId == currentUserId ? f.FriendId : f.UserId);
            var friend = friendId == null ? null : await _db.Users.FindAsync(friendId);

            var game = await _db.TicTacToes
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(c => c.Player1Id == currentUserId || c.Player2Id == currentUserId);

            string[] board = game != null
                ? game.BoardState.Split(',')
                : new string[] { "-", "-", "-", "-", "-", "-", "-", "-", "-" };

            var viewModel = new MyPageViewModel
            {
                CurrentUserId = currentUserId,
                FirstName = firstName,
                Friend = friend,
                Game = game,
                Board = board
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public IActionResult PlayMove(int cellIndex)
        {
            var current = GetCurrentUserId();
            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == current || f.FriendId == current);

            if (friendship == null) return RedirectToAction("MyPage");

            var friendId = friendship.UserId == current ? friendship.FriendId : friendship.UserId;

            var game = _db.TicTacToes.OrderByDescending(c => c.Id).FirstOrDefault(c =>
                 (c.Player1Id == current || c.Player2Id == current));

            if (game != null && game.IsGameOver)
            {
                return RedirectToAction("MyPage");
            }

            if (game == null)
            {
                game = new TicTacToe
                {
                    Player1Id = current,
                    Player2Id = friendId,
                    CurrentTurnPlayerId = friendId,
                    IsGameOver = false,
                    BoardState = "-,-,-,-,-,-,-,-,-"
                };

                string[] board = game.BoardState.Split(',');
                board[cellIndex] = "X";

                game.BoardState = string.Join(",", board);

                _db.TicTacToes.Add(game);
                _db.SaveChanges();
            }
            else
            {

                if (game.CurrentTurnPlayerId != current) return RedirectToAction("MyPage");

                string[] board = game.BoardState.Split(',');


                if (board[cellIndex] != "-") return RedirectToAction("MyPage");


                string znak = game.Player1Id == current ? "X" : "O";

                board[cellIndex] = znak;

                if ((board[0] == znak && board[1] == znak && board[2] == znak) ||
                    (board[3] == znak && board[4] == znak && board[5] == znak) ||
                    (board[6] == znak && board[7] == znak && board[8] == znak) ||
                    (board[0] == znak && board[3] == znak && board[6] == znak) ||
                    (board[1] == znak && board[4] == znak && board[7] == znak) ||
                    (board[2] == znak && board[5] == znak && board[8] == znak) ||
                    (board[0] == znak && board[4] == znak && board[8] == znak) ||
                    (board[2] == znak && board[4] == znak && board[6] == znak))
                {
                    game.WinnerId = current;
                    game.IsGameOver = true;
                }
                else if (!board.Contains("-"))
                {
                    game.WinnerId = "Draw";
                    game.IsGameOver = true;
                }

                game.BoardState = string.Join(",", board);
                game.CurrentTurnPlayerId = game.Player1Id == current ? game.Player2Id : game.Player1Id;

                _db.SaveChanges();
            }

            return RedirectToAction("MyPage");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public IActionResult Restart()
        {
            var current = GetCurrentUserId();
            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == current || f.FriendId == current);

            if (friendship == null) return RedirectToAction("MyPage");
            var friendId = friendship.UserId == current ? friendship.FriendId : friendship.UserId;
            var igra = _db.TicTacToes.OrderByDescending(c => c.Id).FirstOrDefault(x => x.Player1Id == current || x.Player2Id == current);
            if(igra != null)
            {
                _db.TicTacToes.Remove(igra);
                _db.SaveChanges();
            }
                

            var game = new TicTacToe
            {
                Player1Id = current,
                Player2Id = friendId,
                CurrentTurnPlayerId = current,
                IsGameOver = false,
                BoardState = "-,-,-,-,-,-,-,-,-"
            };
            _db.TicTacToes.Add(game);
            _db.SaveChanges();
            return RedirectToAction("MyPage");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckTurn(int gameId, bool isGameOver)
        {
            var current = GetCurrentUserId(); 

            var latestGame = _db.TicTacToes
                .OrderByDescending(c => c.Id)
                .FirstOrDefault(c => c.Player1Id == current || c.Player2Id == current);

            if (latestGame == null) return Json(false);

            if (latestGame.Id != gameId) return Json(true);

            if (latestGame.IsGameOver != isGameOver) return Json(true);

            if (!latestGame.IsGameOver && latestGame.CurrentTurnPlayerId == current) return Json(true);

            return Json(false);
        }
    }



}
