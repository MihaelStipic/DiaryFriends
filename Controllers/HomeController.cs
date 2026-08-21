
using DiaryFriends.Data;
using DiaryFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
        public IActionResult MyPage()
        {
            var currentUserId = GetCurrentUserId();

            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);

            ApplicationUser? friend = null;
            if (friendship != null)
            {
                var friendId = friendship.UserId == currentUserId ? friendship.FriendId : friendship.UserId;
                friend = _db.Users.FirstOrDefault(u => u.Id == friendId);
            }

            ViewBag.Friend = friend;

            return View();
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
    }
}
