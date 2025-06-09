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
                var users = _context.Users
                    .OrderByDescending(u => u.IsActive)
                    .ThenBy(u => u.Username)
                    .ToList();
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
            // Default to no selection
            ViewBag.UserType = null;
            ViewBag.JobSeeker = new JobSeeker();
            ViewBag.Employer = new Employer();

            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user, string userType, JobSeeker jobSeeker = null, Employer employer = null)
        {
            try
            {
                // Validate user type selection
                if (string.IsNullOrEmpty(userType))
                {
                    ModelState.AddModelError("userType", "Please select a user type.");
                }

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

                // Validate required fields based on userType
                if (userType == "JobSeeker" && (jobSeeker == null || jobSeeker.DateOfBirth == default))
                {
                    ModelState.AddModelError("jobSeeker.DateOfBirth", "Date of Birth is required for Job Seekers.");
                }
                else if (userType == "Employer")
                {
                    if (employer == null || string.IsNullOrEmpty(employer.CompanyName))
                    {
                        ModelState.AddModelError("employer.CompanyName", "Company Name is required for Employers.");
                    }
                    if (employer == null || string.IsNullOrEmpty(employer.Industry))
                    {
                        ModelState.AddModelError("employer.Industry", "Industry is required for Employers.");
                    }
                    if (employer == null || employer.FoundedYear <= 0)
                    {
                        ModelState.AddModelError("employer.FoundedYear", "Founded Year is required for Employers.");
                    }
                    if (employer == null || employer.CompanySize <= 0)
                    {
                        ModelState.AddModelError("employer.CompanySize", "Company Size is required for Employers.");
                    }
                }

                if (ModelState.IsValid)
                {
                    user.CreatedAt = DateTime.Now;

                    // Create User without navigation properties first
                    user.JobSeeker = null;
                    user.Employer = null;
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    // Create JobSeeker or Employer based on userType
                    if (userType == "JobSeeker")
                    {
                        if (jobSeeker != null)
                        {
                            jobSeeker.UserId = user.Id;
                            // Ensure non-nullable string properties have default values
                            jobSeeker.Gender = jobSeeker.Gender ?? string.Empty;
                            jobSeeker.ProfilePicture = jobSeeker.ProfilePicture ?? string.Empty;
                            jobSeeker.Headline = jobSeeker.Headline ?? string.Empty;
                            jobSeeker.Summary = jobSeeker.Summary ?? string.Empty;
                            jobSeeker.Skills = jobSeeker.Skills ?? string.Empty;
                            jobSeeker.Education = jobSeeker.Education ?? string.Empty;
                            jobSeeker.WorkExperience = jobSeeker.WorkExperience ?? string.Empty;

                            _context.JobSeekers.Add(jobSeeker);
                            _context.SaveChanges();
                        }
                    }
                    else if (userType == "Employer")
                    {
                        if (employer != null)
                        {
                            employer.UserId = user.Id;
                            // Ensure non-nullable string properties have default values
                            employer.CompanyName = employer.CompanyName ?? string.Empty;
                            employer.CompanyDescription = employer.CompanyDescription ?? string.Empty;
                            employer.Industry = employer.Industry ?? string.Empty;
                            employer.Website = employer.Website ?? string.Empty;
                            employer.Logo = employer.Logo ?? string.Empty;

                            _context.Employers.Add(employer);
                            _context.SaveChanges();
                        }
                    }

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

                // Pass the userType back to the view to maintain the selected option
                ViewBag.UserType = userType;
                ViewBag.JobSeeker = jobSeeker;
                ViewBag.Employer = employer;

                return View(user);
            }
            catch (Exception ex)
            {
                // Log the detailed exception
                Console.WriteLine($"Error creating user: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                ModelState.AddModelError("", $"Error creating user: {ex.Message}");

                // Pass the userType back to the view to maintain the selected option
                ViewBag.UserType = userType;
                ViewBag.JobSeeker = jobSeeker;
                ViewBag.Employer = employer;

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

                // Determine user type
                ViewBag.UserType = user.JobSeeker != null ? "JobSeeker" : (user.Employer != null ? "Employer" : "");

                // Pass JobSeeker/Employer data if exists
                ViewBag.JobSeeker = user.JobSeeker;
                ViewBag.Employer = user.Employer;

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
        public IActionResult Edit(User user, string userType, JobSeeker jobSeeker, Employer employer)
        {
            try
            {
                // Prevent username change
                var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null) return NotFound();

                // Check duplicate email (exclude self)
                if (_context.Users.Any(u => u.Email == user.Email && u.Id != user.Id))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                }

                if (ModelState.IsValid)
                {
                    // Username is not editable
                    // existingUser.Username = user.Username;

                    existingUser.Password = user.Password;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Address = user.Address;
                    existingUser.City = user.City;
                    existingUser.IsActive = user.IsActive;

                    // Handle JobSeeker/Employer update
                    if (userType == "JobSeeker")
                    {
                        // Remove Employer if exists
                        var emp = _context.Employers.FirstOrDefault(e => e.UserId == user.Id);
                        if (emp != null)
                        {
                            _context.Employers.Remove(emp);
                        }

                        var js = _context.JobSeekers.FirstOrDefault(j => j.UserId == user.Id);
                        if (js == null)
                        {
                            js = new JobSeeker { UserId = user.Id };
                            _context.JobSeekers.Add(js);
                        }
                        js.DateOfBirth = jobSeeker.DateOfBirth;
                        js.Gender = jobSeeker.Gender ?? "";
                        js.ProfilePicture = jobSeeker.ProfilePicture ?? "";
                        js.Headline = jobSeeker.Headline ?? "";
                        js.Summary = jobSeeker.Summary ?? "";
                        js.Skills = jobSeeker.Skills ?? "";
                        js.Education = jobSeeker.Education ?? "";
                        js.WorkExperience = jobSeeker.WorkExperience ?? "";
                    }
                    else if (userType == "Employer")
                    {
                        // Remove JobSeeker if exists
                        var js = _context.JobSeekers.FirstOrDefault(j => j.UserId == user.Id);
                        if (js != null)
                        {
                            _context.JobSeekers.Remove(js);
                        }

                        var emp = _context.Employers.FirstOrDefault(e => e.UserId == user.Id);
                        if (emp == null)
                        {
                            emp = new Employer { UserId = user.Id };
                            _context.Employers.Add(emp);
                        }
                        emp.CompanyName = employer.CompanyName ?? "";
                        emp.CompanyDescription = employer.CompanyDescription ?? "";
                        emp.Industry = employer.Industry ?? "";
                        emp.Website = employer.Website ?? "";
                        emp.Logo = employer.Logo ?? "";
                        emp.FoundedYear = employer.FoundedYear;
                        emp.CompanySize = employer.CompanySize;
                    }
                    else
                    {
                        // Remove both if neither selected
                        var js = _context.JobSeekers.FirstOrDefault(j => j.UserId == user.Id);
                        if (js != null) _context.JobSeekers.Remove(js);
                        var emp = _context.Employers.FirstOrDefault(e => e.UserId == user.Id);
                        if (emp != null) _context.Employers.Remove(emp);
                    }

                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                // Repopulate ViewBag for redisplay
                ViewBag.UserType = userType;
                ViewBag.JobSeeker = jobSeeker;
                ViewBag.Employer = employer;
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

                user.IsActive = false;
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