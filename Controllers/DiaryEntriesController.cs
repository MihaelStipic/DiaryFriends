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

        public IActionResult Index(string sortorder)
        {
            var userId = GetCurrentUserId();
            List<DiaryEntry> entries;

            if (sortorder == "dateasc")
            {
                entries = _db.DiaryEntries.Where(x => x.UserId == userId).OrderBy(x => x.Created).ToList();
            }
            else
            {
                entries = _db.DiaryEntries.Where(x => x.UserId == userId).OrderByDescending(x => x.Created).ToList();
            }

            return View(entries);
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
        public IActionResult Show(string sortorder)
        {
            var currentUserId = GetCurrentUserId();

     
            var friendship = _db.Friends.FirstOrDefault(f => f.UserId == currentUserId || f.FriendId == currentUserId);

           
            if (friendship == null)
            {
                ViewBag.User = null;
                return View(new List<DiaryEntry>());
            }

        
            var friendId = friendship.UserId == currentUserId ? friendship.FriendId : friendship.UserId;

           
            var friendUser = _db.Users.FirstOrDefault(u => u.Id == friendId);
            ViewBag.User = friendUser?.FirstName ?? friendUser?.Email;

          
            List<DiaryEntry> entries;

            if (sortorder == "dateasc")
            {
                entries = _db.DiaryEntries.Where(x => x.UserId == friendId).OrderBy(x => x.Created).ToList();
            }
            else
            {
                entries = _db.DiaryEntries.Where(x => x.UserId == friendId).OrderByDescending(x => x.Created).ToList();
            }

            return View(entries);
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


    }
}