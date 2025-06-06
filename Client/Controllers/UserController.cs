using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJobSeekerService _jobSeekerService;
        private readonly IEmployerService _employerService;

        public UserController(
            IUserService userService,
            IJobSeekerService jobSeekerService,
            IEmployerService employerService)
        {
            _userService = userService;
            _jobSeekerService = jobSeekerService;
            _employerService = employerService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (await _userService.ValidateCredentialsAsync(username, password))
            {
                var user = await _userService.GetUserByUsernameAsync(username);

                // Store user ID in session or cookies
                // For simplicity, we'll use TempData for demo purposes
                TempData["UserId"] = user.Id;

                // Explicitly check if user is associated with an employer or job seeker
                var employer = await _employerService.GetEmployerByUserIdAsync(user.Id);
                if (employer != null)
                {
                    TempData["EmployerId"] = employer.Id;
                    return RedirectToAction("Dashboard", "Employer");
                }

                var jobSeeker = await _jobSeekerService.GetJobSeekerByUserIdAsync(user.Id);
                if (jobSeeker != null)
                {
                    TempData["JobSeekerId"] = jobSeeker.Id;
                    return RedirectToAction("Dashboard", "JobSeeker");
                }

                // If no specific role is found, redirect to Admin dashboard
                return RedirectToAction("Dashboard", "Admin");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterJobSeeker(User user, JobSeeker jobSeeker)
        {
            // Validate unique username and email
            if (!await _userService.IsUsernameUniqueAsync(user.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View("Register");
            }

            if (!await _userService.IsEmailUniqueAsync(user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View("Register");
            }

            // Create user (password will be hashed in the service)
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            var createdUser = await _userService.CreateUserAsync(user);

            // Create job seeker profile
            jobSeeker.UserId = createdUser.Id;
            await _jobSeekerService.CreateJobSeekerAsync(jobSeeker);

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployer(User user, Employer employer)
        {
            // Validate unique username and email
            if (!await _userService.IsUsernameUniqueAsync(user.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View("Register");
            }

            if (!await _userService.IsEmailUniqueAsync(user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered");
                return View("Register");
            }

            // Create user (password will be hashed in the service)
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            var createdUser = await _userService.CreateUserAsync(user);

            // Create employer profile
            employer.UserId = createdUser.Id;
            await _employerService.CreateEmployerAsync(employer);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            // Clear user session
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(user);
                return RedirectToAction("Profile", new { id = user.Id });
            }
            return View("Profile", user);
        }
    }
}