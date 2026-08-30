using DiaryFriends.Data;
using DiaryFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiaryFriends.Controllers
{
    [Authorize]
    public class DiaryEntriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DiaryEntriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        public async Task<IActionResult> Index(string sortorder)
        {
            var userId = GetCurrentUserId();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            List<DiaryEntry> entries;

            if (sortorder == "dateasc")
            {
                entries = await _db.DiaryEntries.Include(x => x.reacted).Where(x => x.UserId == userId).OrderBy(x => x.Created).ToListAsync();
            }
            else
            {
                entries = await _db.DiaryEntries.Include(x => x.reacted).Where(x => x.UserId == userId).OrderByDescending(x => x.Created).ToListAsync();
            }

            int displayStreak = user != null ? user.StreakCount : 0;

            if (user != null)
            {
                var today = DateTime.Now.Date;
                var lastEntry = user.StreakDate.Date;
                if (lastEntry < today.AddDays(-1))
                {
                    displayStreak = 0;
                }
            }

            var vm = new DiaryIndexViewModel
            {
                Entries = entries,
                FirstName = user?.FirstName ?? User.Identity?.Name,
                StreakCount = displayStreak
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            var userId = GetCurrentUserId();
            var entry = new DiaryEntry
            {
                UserId = userId,
                Created = DateTime.Now
            };

            return View(entry);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(DiaryEntry obj)
        {
            
            obj.UserId = GetCurrentUserId();
            

            if (!string.IsNullOrEmpty(obj.Title) && obj.Title.Length < 3)
            {
                ModelState.AddModelError("Title", "Title too short");
            }
            //streak logic
            var user = _db.Users.Find(obj.UserId);
            if (user != null)
            {
                var today = DateTime.Now.Date;
                var lastEntryDate = user.StreakDate.Date;

                if (lastEntryDate == today.AddDays(-1))
                {
                    user.StreakCount += 1;
                    user.StreakDate = DateTime.Now;
                }
                else if (lastEntryDate < today.AddDays(-1))
                {
                    user.StreakCount = 1;
                    user.StreakDate = DateTime.Now;
                }
            }

            if (ModelState.IsValid)
            {
                obj.Id = 0;
                _db.DiaryEntries.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaryEntry = _db.DiaryEntries.Find(id);

            if (diaryEntry == null || diaryEntry.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(diaryEntry);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(DiaryEntry obj)
        {
            
            var existingEntry = _db.DiaryEntries.AsNoTracking().FirstOrDefault(x => x.Id == obj.Id);

            if (existingEntry == null || existingEntry.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            
            obj.UserId = GetCurrentUserId();

            if (!string.IsNullOrEmpty(obj.Title) && obj.Title.Length < 3)
            {
                ModelState.AddModelError("Title", "Title too short");
            }

            if (ModelState.IsValid)
            {
                _db.DiaryEntries.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaryEntry = _db.DiaryEntries.Find(id);

            if (diaryEntry == null || diaryEntry.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(diaryEntry);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(DiaryEntry obj)
        {
            // provjera korisnika, da li ovaj diary pripada njemu
            var diaryEntry = _db.DiaryEntries.Find(obj.Id);

            if (diaryEntry == null || diaryEntry.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            _db.DiaryEntries.Remove(diaryEntry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaryEntry = _db.DiaryEntries.Find(id);
            if (diaryEntry == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();

        
            if (diaryEntry.UserId == currentUserId)
            {
                return View(diaryEntry);
            }

    
            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);

            if (friendship != null)
            {
            
                var friendId = (friendship.UserId == currentUserId) ? friendship.FriendId : friendship.UserId;

               
                if (diaryEntry.UserId == friendId)
                {
                    return View(diaryEntry);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Show(string sortorder)
        {
            var currentUserId = GetCurrentUserId();

            var friendship = await _db.Friends.FirstOrDefaultAsync(f => f.UserId == currentUserId || f.FriendId == currentUserId);

            if (friendship == null)
            {
                return View(new DiaryShowViewModel { Entries = new List<DiaryEntry>() });
            }

            var friendId = friendship.UserId == currentUserId ? friendship.FriendId : friendship.UserId;

            var friendUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == friendId);

            List<DiaryEntry> entries;

            if (sortorder == "dateasc")
            {
                entries = await _db.DiaryEntries.Include(x => x.reacted).Where(x => x.UserId == friendId).OrderBy(x => x.Created).ToListAsync();
            }
            else
            {
                entries = await _db.DiaryEntries.Include(x => x.reacted).Where(x => x.UserId == friendId).OrderByDescending(x => x.Created).ToListAsync();
            }

            var vm = new DiaryShowViewModel
            {
                Entries = entries,
                FriendName = friendUser?.FirstName ?? friendUser?.Email,
                StreakCount = friendUser?.StreakCount ?? 0,
                StreakDate = friendUser?.StreakDate
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult ShowView(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diaryEntry = _db.DiaryEntries.Find(id);
            if (diaryEntry == null)
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();


            if (diaryEntry.UserId == currentUserId)
            {
                return View(diaryEntry);
            }


            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);

            if (friendship != null)
            {

                var friendId = (friendship.UserId == currentUserId) ? friendship.FriendId : friendship.UserId;


                if (diaryEntry.UserId == friendId)
                {
                    return View(diaryEntry);
                }
            }

            return RedirectToAction("Index");
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AddReaction(int entryId, string reactionType)
        {
            var currentUserId = GetCurrentUserId();

            var diaryEntry = _db.DiaryEntries.Find(entryId);
            if (diaryEntry == null)
            {
                return NotFound();
            }

            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);
            bool isFriend = friendship != null &&
                diaryEntry.UserId == (friendship.UserId == currentUserId ? friendship.FriendId : friendship.UserId);

            if (!isFriend)
            {
                return NotFound();
            }

            var existingReaction = _db.Reactions.FirstOrDefault(x => x.DiaryEntryId == entryId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == reactionType)
                {
                    _db.Reactions.Remove(existingReaction);
                    _db.SaveChanges();
                    return RedirectToAction("Show");
                }
                else
                {
                    existingReaction.ReactionType = reactionType;
                    _db.SaveChanges();
                }
            }
            else
            {
                var react = new Reaction
                {
                    DiaryEntryId = entryId,
                    UserId = currentUserId,
                    ReactionType = reactionType
                };

                _db.Reactions.Add(react);
                _db.SaveChanges();
            }

            return RedirectToAction("Show");
        }


    }
}