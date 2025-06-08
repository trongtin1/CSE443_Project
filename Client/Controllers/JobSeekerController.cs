using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace CSE443_Project.Controllers
{
    public class JobSeekerController : Controller
    {
        private readonly IJobSeekerService _jobSeekerService;
        private readonly IUserService _userService;
        private readonly IApplicationService _applicationService;
        private readonly ISaveJobService _saveJobService;
        private readonly ICVService _cvService;
        private readonly ICandidateService _candidateService; public JobSeekerController(
            IJobSeekerService jobSeekerService,
            IUserService userService,
            IApplicationService applicationService,
            ISaveJobService saveJobService,
            ICVService cvService,
            ICandidateService candidateService)
        {
            _jobSeekerService = jobSeekerService;
            _userService = userService;
            _applicationService = applicationService;
            _saveJobService = saveJobService;
            _cvService = cvService;
            _candidateService = candidateService;
        }
        private int GetJobSeekerIdFromTempData()
        {
            if (TempData["JobSeekerId"] is int jobSeekerId)
                return jobSeekerId;
            return 0;
        }// GET: /JobSeeker/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            ViewBag.ApplicationCount = await _applicationService.GetApplicationCountByJobSeekerIdAsync(jobSeekerId);
            var savedJobs = await _saveJobService.GetSaveJobsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.SavedJobCount = savedJobs.Count();
            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.CVCount = cvs.Count();
            ViewBag.ShortlistedCount = (await _candidateService.GetCandidatesByJobSeekerIdAsync(jobSeekerId)).Count();

            return View(jobSeeker);
        }        // GET: /JobSeeker/Profile
        public async Task<IActionResult> Profile()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }        // POST: /JobSeeker/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(JobSeeker jobSeeker)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }
            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            if (jobSeeker.Id != jobSeekerId)
            {
                return Forbid();
            }

            // Remove validation for navigation properties that aren't being updated
            ModelState.Remove("User");
            ModelState.Remove("Applications");
            ModelState.Remove("SavedJobs");
            ModelState.Remove("CVs");
            ModelState.Remove("Candidacies");

            // Add debugging to check ModelState
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                // Create detailed error message
                var errorDetails = string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors ?? new string[0])}"));
                TempData["ErrorMessage"] = $"Validation failed. Errors: {errorDetails}";                // If validation fails, get the full JobSeeker object with navigation properties
                var fullJobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
                if (fullJobSeeker == null)
                {
                    return NotFound();
                }

                // Copy the form values to the full object
                fullJobSeeker.DateOfBirth = jobSeeker.DateOfBirth;
                fullJobSeeker.Gender = jobSeeker.Gender;
                fullJobSeeker.Headline = jobSeeker.Headline;
                fullJobSeeker.Summary = jobSeeker.Summary;
                fullJobSeeker.Skills = jobSeeker.Skills;
                fullJobSeeker.Education = jobSeeker.Education;
                fullJobSeeker.WorkExperience = jobSeeker.WorkExperience;
                // Keep existing ProfilePicture - don't overwrite it

                return View("Profile", fullJobSeeker);
            }
            try
            {
                // Get the existing JobSeeker to preserve ProfilePicture and other properties
                var existingJobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
                if (existingJobSeeker == null)
                {
                    return NotFound();
                }

                // Update only the fields that can be modified from the form
                existingJobSeeker.DateOfBirth = jobSeeker.DateOfBirth;
                existingJobSeeker.Gender = jobSeeker.Gender;
                existingJobSeeker.Headline = jobSeeker.Headline;
                existingJobSeeker.Summary = jobSeeker.Summary;
                existingJobSeeker.Skills = jobSeeker.Skills;
                existingJobSeeker.Education = jobSeeker.Education;
                existingJobSeeker.WorkExperience = jobSeeker.WorkExperience;

                // Keep the existing ProfilePicture if it's not provided in the form
                if (string.IsNullOrEmpty(jobSeeker.ProfilePicture))
                {
                    // Don't update ProfilePicture, keep the existing one
                }
                else
                {
                    existingJobSeeker.ProfilePicture = jobSeeker.ProfilePicture;
                }

                await _jobSeekerService.UpdateJobSeekerAsync(existingJobSeeker);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please try again.";

                // Get the full JobSeeker object with navigation properties
                var fullJobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
                if (fullJobSeeker == null)
                {
                    return NotFound();
                }

                // Copy the form values to the full object
                fullJobSeeker.DateOfBirth = jobSeeker.DateOfBirth;
                fullJobSeeker.Gender = jobSeeker.Gender;
                fullJobSeeker.Headline = jobSeeker.Headline;
                fullJobSeeker.Summary = jobSeeker.Summary;
                fullJobSeeker.Skills = jobSeeker.Skills;
                fullJobSeeker.Education = jobSeeker.Education;
                fullJobSeeker.WorkExperience = jobSeeker.WorkExperience;

                return View("Profile", fullJobSeeker);
            }
        }

        // POST: /JobSeeker/UploadProfilePicture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return Json(new { success = false, message = "User session expired. Please log in again." });
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            if (profilePicture == null || profilePicture.Length == 0)
            {
                return Json(new { success = false, message = "Please select an image file." });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Json(new { success = false, message = "Only JPG, PNG, and GIF files are allowed." });
            }

            // Validate file size (5MB max)
            if (profilePicture.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "File size cannot exceed 5MB." });
            }

            try
            {
                // Get current job seeker
                var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
                if (jobSeeker == null)
                {
                    return Json(new { success = false, message = "Job seeker not found." });
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pictures");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                // Update job seeker profile picture path
                jobSeeker.ProfilePicture = "/uploads/profile-pictures/" + uniqueFileName;
                await _jobSeekerService.UpdateJobSeekerAsync(jobSeeker);

                return Json(new { success = true, message = "Profile picture updated successfully!", imageUrl = jobSeeker.ProfilePicture });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while uploading the image. Please try again." });
            }
        }

        // GET: /JobSeeker/SavedJobs
        public async Task<IActionResult> SavedJobs()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var savedJobs = await _saveJobService.GetSaveJobsByJobSeekerIdAsync(jobSeekerId);
            return View(savedJobs);
        }

        // POST: /JobSeeker/SaveJob/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveJob(int id, string? notes = null)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            // Check if the job is already saved
            if (await _saveJobService.HasJobSeekerSavedJobAsync(jobSeekerId, id))
            {
                return RedirectToAction("Details", "Job", new { id });
            }

            var saveJob = new SaveJob
            {
                JobSeekerId = jobSeekerId,
                JobId = id,
                SavedAt = DateTime.Now,
                Notes = notes
            };

            await _saveJobService.CreateSaveJobAsync(saveJob);
            return RedirectToAction("Details", "Job", new { id });
        }

        // POST: /JobSeeker/UnsaveJob/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsaveJob(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var savedJob = await _saveJobService.GetSaveJobByJobSeekerAndJobIdAsync(jobSeekerId, id);
            if (savedJob != null)
            {
                await _saveJobService.DeleteSaveJobAsync(savedJob.Id);
            }

            return RedirectToAction("Details", "Job", new { id });
        }

        // GET: /JobSeeker/CVs
        public async Task<IActionResult> CVs()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            TempData.Keep("JobSeekerId");

            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            return View(cvs);
        }

        // GET: /JobSeeker/CreateCV
        public IActionResult CreateCV()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            return View();
        }        // POST: /JobSeeker/CreateCV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCV(CV cv, IFormFile cvFile)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            // Remove validation for navigation properties that aren't being updated
            ModelState.Remove("JobSeeker"); if (ModelState.IsValid)
            {
                cv.JobSeekerId = jobSeekerId;
                cv.CreatedAt = DateTime.Now;

                // Process the uploaded file
                if (cvFile != null && cvFile.Length > 0)
                {
                    // Create directory if it doesn't exist
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string uniqueFileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await cvFile.CopyToAsync(fileStream);
                    }

                    // Update the CV model with the file path
                    cv.FilePath = $"/uploads/cvs/{uniqueFileName}";
                }
                await _cvService.CreateCVAsync(cv);
                return RedirectToAction(nameof(CVs));
            }

            // Add debugging to check ModelState
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                // Create detailed error message
                var errorDetails = string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors ?? new string[0])}"));
                TempData["ErrorMessage"] = $"Validation failed. Errors: {errorDetails}";
            }

            return View(cv);
        }

        // GET: /JobSeeker/EditCV/5
        public async Task<IActionResult> EditCV(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var cv = await _cvService.GetCVByIdAsync(id);
            if (cv == null)
            {
                return NotFound();
            }

            // Ensure the CV belongs to this job seeker
            if (cv.JobSeekerId != jobSeekerId)
            {
                return Forbid();
            }

            return View(cv);
        }        // POST: /JobSeeker/EditCV/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCV(int id, CV cv, IFormFile cvFile)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            if (id != cv.Id)
            {
                return NotFound();
            }

            // Get the existing CV to maintain the FilePath if no new file is uploaded
            var existingCV = await _cvService.GetCVByIdAsync(id);
            if (existingCV == null)
            {
                return NotFound();
            }

            // Ensure the CV belongs to this job seeker
            if (existingCV.JobSeekerId != jobSeekerId)
            {
                return Forbid();
            }            // Remove validation for navigation properties that aren't being updated
            ModelState.Remove("JobSeeker");
            // Remove validation for cvFile since it's not required for edit
            ModelState.Remove("cvFile");

            // Validate file if uploaded
            if (cvFile != null && cvFile.Length > 0)
            {
                // Validate file size (5MB max)
                if (cvFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("cvFile", "File size cannot exceed 5MB.");
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(cvFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("cvFile", "Only PDF, DOC, and DOCX files are allowed.");
                }
            }            // Maintain the existing FilePath if no new file is uploaded
            if (cvFile == null || cvFile.Length == 0)
            {
                // Keep existing file path - no change needed since we're updating existingCV
            }
            else
            {
                // Process the uploaded file
                if (cvFile.Length > 0)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(existingCV.FilePath))
                    {
                        string oldFilePath = existingCV.FilePath.TrimStart('/');
                        string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldFilePath);
                        if (System.IO.File.Exists(oldFullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFullPath);
                            }
                            catch (Exception ex)
                            {
                                // Log the error but don't stop the process
                                Console.WriteLine($"Error deleting old file: {ex.Message}");
                            }
                        }
                    }

                    // Create directory if it doesn't exist
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string uniqueFileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await cvFile.CopyToAsync(fileStream);
                    }

                    // Update the existing CV's file path
                    existingCV.FilePath = $"/uploads/cvs/{uniqueFileName}";
                }
            }
            if (ModelState.IsValid)
            {
                // Update the existing tracked entity instead of creating a new one
                existingCV.Title = cv.Title;
                existingCV.Description = cv.Description;
                existingCV.IsDefault = cv.IsDefault;
                // Don't update FilePath here as it's already updated above if new file was uploaded
                existingCV.UpdatedAt = DateTime.Now;

                await _cvService.UpdateCVAsync(existingCV);
                TempData["SuccessMessage"] = "CV updated successfully!";
                return RedirectToAction(nameof(CVs));
            }

            // Add debugging to check ModelState
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                // Create detailed error message
                var errorDetails = string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors ?? new string[0])}"));
                TempData["ErrorMessage"] = $"Validation failed. Errors: {errorDetails}";
            }

            return View(cv);
        }

        // POST: /JobSeeker/DeleteCV/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCV(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var cv = await _cvService.GetCVByIdAsync(id);
            if (cv == null)
            {
                return NotFound();
            }

            // Ensure the CV belongs to this job seeker
            if (cv.JobSeekerId != jobSeekerId)
            {
                return Forbid();
            }

            await _cvService.DeleteCVAsync(id);
            return RedirectToAction(nameof(CVs));
        }

        // POST: /JobSeeker/SetDefaultCV/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultCV(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            await _cvService.SetDefaultCVAsync(id, jobSeekerId);
            return RedirectToAction(nameof(CVs));
        }

        // GET: /JobSeeker/Applications
        public async Task<IActionResult> Applications()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var applications = await _applicationService.GetApplicationsByJobSeekerIdAsync(jobSeekerId);
            return View(applications);
        }

        // GET: /JobSeeker/Candidates
        public async Task<IActionResult> Candidates()
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var candidates = await _candidateService.GetCandidatesByJobSeekerIdAsync(jobSeekerId);
            return View(candidates);
        }

        // GET: /JobSeeker/CandidateDetails/5
        public async Task<IActionResult> CandidateDetails(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var candidate = await _candidateService.GetCandidateByIdAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            // Ensure the candidate belongs to this job seeker
            if (candidate.JobSeekerId != jobSeekerId)
            {
                return Forbid();
            }

            // Add JobSeekerId to ViewBag for SignalR connection
            ViewBag.JobSeekerId = jobSeekerId;

            return View(candidate);
        }

        // GET: /JobSeeker/DownloadCV/5
        public async Task<IActionResult> DownloadCV(int id)
        {
            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            var cv = await _cvService.GetCVByIdAsync(id);
            if (cv == null)
            {
                return NotFound();
            }

            // Ensure the CV belongs to this job seeker
            if (cv.JobSeekerId != jobSeekerId)
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(cv.FilePath))
            {
                return NotFound("No file available for this CV.");
            }

            // Remove leading slash if present
            string filePath = cv.FilePath.TrimStart('/');

            // Combine with wwwroot path
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("The file does not exist.");
            }

            // Determine content type
            string contentType = "application/pdf";
            if (filePath.EndsWith(".docx"))
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (filePath.EndsWith(".doc"))
                contentType = "application/msword";

            // Get filename from path
            string fileName = Path.GetFileName(filePath);

            // Return file
            return PhysicalFile(fullPath, contentType, fileName);
        }
    }
}