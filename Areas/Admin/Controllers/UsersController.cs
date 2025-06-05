using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public IActionResult Index()
        {
            try
            {
                var users = _context.Users.ToList();
                return View(users);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fetching users: {ex.Message}");
                
                // Check if error is related to missing table
                if (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message.Contains("Invalid object name") == true)
                {
                    ViewBag.ErrorMessage = "The database schema is not properly set up. Please run migrations to create the required tables.";
                    return View(new List<User>());
                }
                
                // Return empty list with error message for other errors
                ViewBag.ErrorMessage = "An error occurred while fetching users. Please try again later.";
                return View(new List<User>());
            }
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
            try
            {
                // Kiểm tra trùng username
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                }
                // Kiểm tra trùng email
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }

                if (ModelState.IsValid)
                {
                    user.CreatedAt = DateTime.Now;
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                // Log errors to console for debugging
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error in field {key}: {error.ErrorMessage}");
                    }
                }

                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating user: {ex.Message}");
                return View(user);
            }
        }

        // GET: Admin/Users/Edit/5
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                    if (existingUser == null) return NotFound();

                    // Update fields
                    existingUser.Username = user.Username;
                    existingUser.Password = user.Password;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Address = user.Address;
                    existingUser.City = user.City;
                    existingUser.IsActive = user.IsActive;

                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                return View(user);
            }
        }

        // GET: Admin/Users/Delete/5
        public IActionResult Delete(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                _context.Users.Remove(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error deleting user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Users/Details/5
        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null) return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading user details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}