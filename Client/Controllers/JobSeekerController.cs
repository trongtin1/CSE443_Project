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
        private readonly ICandidateService _candidateService;

        public JobSeekerController(
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

        // GET: /JobSeeker/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            ViewBag.ApplicationCount = await _applicationService.GetApplicationCountByJobSeekerIdAsync(jobSeekerId);
            ViewBag.SavedJobCount = await _saveJobService.GetSaveJobsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.CVCount = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.ShortlistedCount = (await _candidateService.GetCandidatesByJobSeekerIdAsync(jobSeekerId)).Count();

            return View(jobSeeker);
        }

        // GET: /JobSeeker/Profile
        public async Task<IActionResult> Profile()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        // POST: /JobSeeker/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(JobSeeker jobSeeker)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            if (jobSeeker.Id != jobSeekerId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                await _jobSeekerService.UpdateJobSeekerAsync(jobSeeker);
                return RedirectToAction(nameof(Profile));
            }

            return View("Profile", jobSeeker);
        }

        // GET: /JobSeeker/SavedJobs
        public async Task<IActionResult> SavedJobs()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var savedJobs = await _saveJobService.GetSaveJobsByJobSeekerIdAsync(jobSeekerId);
            return View(savedJobs);
        }

        // POST: /JobSeeker/SaveJob/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveJob(int id, string notes = null)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            return View(cvs);
        }

        // GET: /JobSeeker/CreateCV
        public IActionResult CreateCV()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            TempData.Keep("JobSeekerId");
            return View();
        }

        // POST: /JobSeeker/CreateCV
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCV(CV cv, IFormFile cvFile, bool isDefault = false)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            if (ModelState.IsValid)
            {
                cv.JobSeekerId = jobSeekerId;
                cv.IsDefault = isDefault;
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

            return View(cv);
        }

        // GET: /JobSeeker/EditCV/5
        public async Task<IActionResult> EditCV(int id)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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
        }

        // POST: /JobSeeker/EditCV/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCV(int id, CV cv, IFormFile cvFile, bool isDefault = false)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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
            }

            // Maintain the existing FilePath if no new file is uploaded
            if (cvFile == null)
            {
                cv.FilePath = existingCV.FilePath;
            }
            else
            {
                // Process the uploaded file
                if (cvFile.Length > 0)
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

                    // Update the CV model with the new file path
                    cv.FilePath = $"/uploads/cvs/{uniqueFileName}";
                }
            }

            if (ModelState.IsValid)
            {
                cv.JobSeekerId = jobSeekerId; // Ensure JobSeekerId is set
                cv.IsDefault = isDefault;
                cv.UpdatedAt = DateTime.Now;
                cv.CreatedAt = existingCV.CreatedAt; // Maintain original creation date

                await _cvService.UpdateCVAsync(cv);
                return RedirectToAction(nameof(CVs));
            }

            return View(cv);
        }

        // POST: /JobSeeker/DeleteCV/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCV(int id)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            await _cvService.SetDefaultCVAsync(id, jobSeekerId);
            return RedirectToAction(nameof(CVs));
        }

        // GET: /JobSeeker/Applications
        public async Task<IActionResult> Applications()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var applications = await _applicationService.GetApplicationsByJobSeekerIdAsync(jobSeekerId);
            return View(applications);
        }

        // GET: /JobSeeker/Candidates
        public async Task<IActionResult> Candidates()
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            var candidates = await _candidateService.GetCandidatesByJobSeekerIdAsync(jobSeekerId);
            return View(candidates);
        }

        // GET: /JobSeeker/DownloadCV/5
        public async Task<IActionResult> DownloadCV(int id)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

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