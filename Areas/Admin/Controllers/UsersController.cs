using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;
using System.Linq;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.created_at = DateTime.UtcNow;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.user_id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.user_id == user.user_id);
                if (existingUser == null) return NotFound();

                // Cập nhật các trường
                existingUser.name = user.name;
                existingUser.email = user.email;
                existingUser.password = user.password;
                existingUser.phone = user.phone;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.user_id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Users/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var user = _context.Users.FirstOrDefault(u => u.user_id == id);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}
