using DiaryFriends.Data;
using DiaryFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DiaryFriends.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FriendsController(ApplicationDbContext db)
        {
            _db = db;
        }
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }


        public IActionResult Index(string searchQuery)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return View(new List<ApplicationUser>());
            }

            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);

            ApplicationUser? currentFriend = null;
            if (friendship != null)
            {
                var friendId = friendship.UserId == currentUserId ? friendship.FriendId : friendship.UserId;
                currentFriend = _db.Users.FirstOrDefault(u => u.Id == friendId);
            }

            ViewBag.CurrentFriend = currentFriend;

            
            if (currentFriend != null)
            {
                return View(new List<ApplicationUser>());
            }

            
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var query = searchQuery.ToLower();

                var users = _db.Users
                    .Where(u => u.Id != currentUserId)
                    .Where(u => !_db.Friends.Any(f => f.UserId == u.Id || f.FriendId == u.Id))
                    .Where(u => (u.Email != null && u.Email.ToLower().Contains(query)) ||
                                (u.FirstName != null && u.FirstName.ToLower().Contains(query)))
                    .ToList();

                return View(users);
            }

            return View(new List<ApplicationUser>());
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Add(string friendId)
        {
            var currentUserId = GetCurrentUserId();

            // Delete friend if they deleted their account - fix!!
            var deadFriendships = _db.Friends.Where(f => !_db.Users.Any(u => u.Id == f.UserId) || !_db.Users.Any(u => u.Id == f.FriendId)).ToList();
            if (deadFriendships.Any())
            {
                var deadReactions = _db.Reactions
                .Where(r => !_db.Users.Any(u => u.Id == r.UserId))
                .ToList();

                _db.Reactions.RemoveRange(deadReactions);
                _db.Friends.RemoveRange(deadFriendships);
                _db.SaveChanges();
            }

            
            bool hasFriend = _db.Friends.Any(f => f.UserId == currentUserId || f.FriendId == currentUserId ||
                                                  f.UserId == friendId || f.FriendId == friendId);

            if (!hasFriend && !string.IsNullOrEmpty(friendId) && friendId != currentUserId)
            {
                var newFriendship = new Friend
                {
                    UserId = currentUserId,
                    FriendId = friendId
                };

                _db.Friends.Add(newFriendship);
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Remove(string friendId)
        {
            var currentUserId = GetCurrentUserId();

            var friendship = _db.Friends.FirstOrDefault(f =>
                (f.UserId == currentUserId && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == currentUserId));

            if (friendship != null)
            {
                _db.Friends.Remove(friendship);
                
                _db.SaveChanges();
            }
            List<Reaction> reactions = _db.Reactions.Where(x => x.UserId == currentUserId || x.UserId == friendId).ToList();
            foreach (var react in reactions)
            {
                _db.Reactions.Remove(react);
                
            }
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
