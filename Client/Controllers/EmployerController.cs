using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Controllers
{
    public class EmployerController : Controller
    {
        private readonly IEmployerService _employerService;
        private readonly IUserService _userService;
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;
        private readonly ICandidateService _candidateService;

        public EmployerController(
            IEmployerService employerService,
            IUserService userService,
            IJobService jobService,
            IApplicationService applicationService,
            ICandidateService candidateService)
        {
            _employerService = employerService;
            _userService = userService;
            _jobService = jobService;
            _applicationService = applicationService;
            _candidateService = candidateService;
        }        // GET: /Employer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var employer = await _employerService.GetEmployerByIdAsync(employerId);
            if (employer == null)
            {
                return NotFound();
            }

            ViewBag.ActiveJobCount = await _employerService.GetActiveJobCountByEmployerIdAsync(employerId);
            ViewBag.TotalJobCount = await _employerService.GetJobCountByEmployerIdAsync(employerId);

            // Get application statistics
            var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
            ViewBag.TotalApplications = applications.Count();
            ViewBag.PendingApplications = applications.Count(a => a.Status == "Pending");
            ViewBag.ShortlistedApplications = applications.Count(a => a.Status == "Shortlisted");

            // Get recent applications
            ViewBag.RecentApplications = applications.OrderByDescending(a => a.ApplicationDate).Take(5);

            // Get active jobs
            var activeJobs = await _jobService.GetJobsByEmployerIdAsync(employerId);
            ViewBag.ActiveJobs = activeJobs.Where(j => j.IsActive && j.Deadline >= DateTime.Now).Take(5);

            return View(employer);
        }        // GET: /Employer/Profile
        public async Task<IActionResult> Profile()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var employer = await _employerService.GetEmployerByIdAsync(employerId);
            if (employer == null)
            {
                return NotFound();
            }

            // Get statistics for the profile page
            ViewBag.ActiveJobCount = await _employerService.GetActiveJobCountByEmployerIdAsync(employerId);
            ViewBag.TotalJobCount = await _employerService.GetJobCountByEmployerIdAsync(employerId);

            var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
            ViewBag.TotalApplications = applications.Count();

            return View(employer);
        }        // POST: /Employer/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Employer employer, IFormFile? logoFile)
        {
            int? employerId = null;

            try
            {
                // Ensure the user is an employer
                if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
                {
                    return RedirectToAction("Login", "User");
                }

                employerId = GetEmployerIdFromTempData();
                if (employerId == null)
                {
                    return RedirectToAction("Login", "User");
                }

                TempData.Keep("EmployerId");

                if (employer.Id != employerId)
                {
                    return Forbid();
                }

                // Remove navigation properties from model validation to prevent validation errors
                ModelState.Remove("User");
                ModelState.Remove("Jobs");
                ModelState.Remove("Applications");

                // If no new logo file is uploaded, preserve the existing logo
                if (logoFile == null || logoFile.Length == 0)
                {
                    var existingEmployer = await _employerService.GetEmployerByIdAsync(employerId.Value);
                    if (existingEmployer != null && !string.IsNullOrEmpty(existingEmployer.Logo))
                    {
                        employer.Logo = existingEmployer.Logo;
                    }
                }

                if (ModelState.IsValid)
                {
                    // Handle logo upload only if a new file is provided
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(logoFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("logoFile", "Only JPG, PNG, and GIF files are allowed.");
                            await LoadEmployerViewBag(employerId.Value);
                            return View("Profile", employer);
                        }

                        // Validate file size (5MB max)
                        if (logoFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("logoFile", "File size cannot exceed 5MB.");
                            await LoadEmployerViewBag(employerId.Value);
                            return View("Profile", employer);
                        }

                        // Save the file
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(fileStream);
                        }

                        employer.Logo = "/uploads/logos/" + uniqueFileName;
                    }

                    await _employerService.UpdateEmployerAsync(employer);
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }
                else
                {
                    // Debug: Log validation errors
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Key = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    TempData["ErrorMessage"] = "Please correct the validation errors and try again.";
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error for {error.Key}: {string.Join(", ", error.Errors ?? new List<string>())}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details for debugging
                Console.WriteLine($"Error updating employer profile: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Log inner exception if present
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";

                // For debugging purposes, you can also add the actual error message
                // TempData["ErrorMessage"] = $"An error occurred while updating your profile: {ex.Message}";
            }

            // If we got this far, something failed, redisplay form
            await LoadEmployerViewBag(employerId ?? employer.Id);
            return View("Profile", employer);
        }

        // Helper method to safely get employer ID from TempData
        private int? GetEmployerIdFromTempData()
        {
            if (TempData.ContainsKey("EmployerId") && TempData["EmployerId"] != null)
            {
                if (TempData["EmployerId"] is int id)
                {
                    return id;
                }
                if (int.TryParse(TempData["EmployerId"]?.ToString(), out int parsedId))
                {
                    return parsedId;
                }
            }
            return null;
        }

        // Helper method to load ViewBag data for profile page
        private async Task LoadEmployerViewBag(int employerId)
        {
            ViewBag.ActiveJobCount = await _employerService.GetActiveJobCountByEmployerIdAsync(employerId);
            ViewBag.TotalJobCount = await _employerService.GetJobCountByEmployerIdAsync(employerId);
            var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
            ViewBag.TotalApplications = applications.Count();
        }// GET: /Employer/Jobs
        public async Task<IActionResult> Jobs()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var jobs = await _jobService.GetJobsByEmployerIdAsync(employerId);
            return View(jobs);
        }

        // GET: /Employer/Applications
        public async Task<IActionResult> Applications()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
            return View(applications);
        }

        // GET: /Employer/ApplicationsByStatus
        public async Task<IActionResult> ApplicationsByStatus(string status)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var applications = await _applicationService.GetApplicationsByStatusAsync(status);

            // Filter applications to only show those for jobs posted by this employer
            var employerApplications = applications.Where(a => a.Job.EmployerId == employerId);

            ViewBag.Status = status;

            return View("Applications", employerApplications);
        }

        // GET: /Employer/ApplicationsByJob/5
        public async Task<IActionResult> ApplicationsByJob(int id)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to this employer
            if (job.EmployerId != employerId)
            {
                return Forbid();
            }

            var applications = await _applicationService.GetApplicationsByJobIdAsync(id);

            ViewBag.Job = job;

            return View("Applications", applications);
        }

        // GET: /Employer/Candidates
        public async Task<IActionResult> Candidates()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            // Get all candidates for jobs posted by this employer
            var allCandidates = await _candidateService.GetAllCandidatesAsync();
            var employerCandidates = allCandidates.Where(c => c.Job.EmployerId == employerId);

            return View(employerCandidates);
        }

        // GET: /Employer/CandidatesByJob/5
        public async Task<IActionResult> CandidatesByJob(int id)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to this employer
            if (job.EmployerId != employerId)
            {
                return Forbid();
            }

            var candidates = await _candidateService.GetCandidatesByJobIdAsync(id);

            ViewBag.Job = job;

            return View("Candidates", candidates);
        }

        // GET: /Employer/CandidatesByStatus
        public async Task<IActionResult> CandidatesByStatus(string status)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var candidates = await _candidateService.GetCandidatesByStatusAsync(status);

            // Filter candidates to only show those for jobs posted by this employer
            var employerCandidates = candidates.Where(c => c.Job.EmployerId == employerId);

            ViewBag.Status = status;

            return View("Candidates", employerCandidates);
        }

        // GET: /Employer/CandidateDetails/5
        public async Task<IActionResult> CandidateDetails(int id)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            // Ensure the candidate is for a job posted by this employer
            if (candidate.Job.EmployerId != employerId)
            {
                return Forbid();
            }

            return View(candidate);
        }

        // POST: /Employer/UpdateCandidateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCandidateStatus(int id, string status, string interviewNotes, DateTime? interviewDate)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            var candidate = await _candidateService.GetCandidateByIdAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            // Ensure the candidate is for a job posted by this employer
            if (candidate.Job.EmployerId != employerId)
            {
                return Forbid();
            }

            await _candidateService.UpdateCandidateStatusAsync(id, status, interviewNotes, interviewDate);

            return RedirectToAction(nameof(CandidateDetails), new { id });
        }

        // POST: /Employer/UploadLogo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(IFormFile logoFile)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId") || TempData["EmployerId"] == null)
            {
                return Json(new { success = false, message = "User session expired. Please log in again." });
            }

            var employerId = (int)TempData["EmployerId"]!;
            TempData.Keep("EmployerId");

            if (logoFile == null || logoFile.Length == 0)
            {
                return Json(new { success = false, message = "Please select a logo file." });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(logoFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Json(new { success = false, message = "Only JPG, PNG, and GIF files are allowed." });
            }

            // Validate file size (5MB max)
            if (logoFile.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "File size cannot exceed 5MB." });
            }

            try
            {
                // Get current employer
                var employer = await _employerService.GetEmployerByIdAsync(employerId);
                if (employer == null)
                {
                    return Json(new { success = false, message = "Employer not found." });
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(fileStream);
                }

                // Update employer logo path
                employer.Logo = "/uploads/logos/" + uniqueFileName;
                await _employerService.UpdateEmployerAsync(employer);

                return Json(new { success = true, message = "Company logo updated successfully!", imageUrl = employer.Logo });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while uploading the logo. Please try again." });
            }
        }
    }
}