using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobService _jobService;
        private readonly IJobSeekerService _jobSeekerService;
        private readonly ICVService _cvService;
        private readonly ICandidateService _candidateService;
        private readonly INotificationService _notificationService;

        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            IJobSeekerService jobSeekerService,
            ICVService cvService,
            ICandidateService candidateService,
            INotificationService notificationService)
        {
            _applicationService = applicationService;
            _jobService = jobService;
            _jobSeekerService = jobSeekerService;
            _cvService = cvService;
            _candidateService = candidateService;
            _notificationService = notificationService;
        }

        // Helper method to extract job ID from URL if route binding fails
        private int ExtractJobIdFromUrl()
        {
            try
            {
                var path = Request.Path.Value;
                if (string.IsNullOrEmpty(path))
                    return 0;

                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length < 3)
                    return 0;

                // The URL format should be /Application/Apply/{id}
                if (segments.Length >= 3 &&
                    segments[0].Equals("Application", StringComparison.OrdinalIgnoreCase) &&
                    segments[1].Equals("Apply", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(segments[2], out int jobId))
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting job ID from URL: {ex.Message}");
                return 0;
            }
        }

        // GET: /Application
        public async Task<IActionResult> Index()
        {
            // Check if user is a job seeker or employer
            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId");
            if (jobSeekerId != null)
            {
                var applications = await _applicationService.GetApplicationsByJobSeekerIdAsync(jobSeekerId.Value);
                return View("JobSeekerApplications", applications);
            }
            else if (HttpContext.Session.GetInt32("EmployerId") != null)
            {
                var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
                var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
                return View("EmployerApplications", applications);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // GET: /Application/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Ensure the user has permission to view this application
            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId");
            if (jobSeekerId != null)
            {
                if (application.JobSeekerId != jobSeekerId)
                {
                    return Forbid();
                }
            }
            else if (HttpContext.Session.GetInt32("EmployerId") != null)
            {
                var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
                if (application.Job.EmployerId != employerId)
                {
                    return Forbid();
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            return View(application);
        }

        // GET: /Application/Apply/5
        public async Task<IActionResult> Apply(int id)
        {
            Console.WriteLine($"=== GET Apply DEBUG ===");
            Console.WriteLine($"Raw route ID: {id}");
            Console.WriteLine($"Request path: {Request.Path}");
            Console.WriteLine($"Request query string: {Request.QueryString}");

            // Try to extract ID from URL if route binding failed
            if (id <= 0)
            {
                id = ExtractJobIdFromUrl();
                Console.WriteLine($"Extracted ID from URL: {id}");
            }

            // Ensure the job ID is valid
            if (id <= 0)
            {
                Console.WriteLine($"Invalid job ID detected: {id}");
                TempData["ErrorMessage"] = "Invalid job ID. Please select a valid job to apply for.";
                return RedirectToAction("Index", "Job");
            }

            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in as a job seeker to apply for jobs.";
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            Console.WriteLine($"GET Apply - Job ID: {id}, JobSeeker ID: {jobSeekerId}");

            // Check if the job seeker has already applied to this job
            if (await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id))
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Details", "Job", new { id });
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Get the job seeker's CVs for selection
            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            Console.WriteLine($"CV Count: {cvs.Count()}");

            ViewBag.CVs = cvs;
            ViewBag.Job = job;

            // Initialize a new application
            var application = new Application
            {
                JobId = id,
                JobSeekerId = jobSeekerId
            };

            return View(application);
        }

        // POST: /Application/Apply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id, Application application, bool? NoCVsAvailable)
        {
            Console.WriteLine("=== APPLICATION SUBMIT DEBUG ===");
            Console.WriteLine($"POST Apply - Job ID: {id}, Application: {application?.CoverLetter?.Length ?? 0} chars, NoCVsAvailable: {NoCVsAvailable}");
            Console.WriteLine($"Application JobId from model: {application?.JobId}");

            // Try to extract ID from URL if route binding failed
            if (id <= 0)
            {
                id = ExtractJobIdFromUrl();
                Console.WriteLine($"Extracted ID from URL for POST: {id}");
            }

            // Ensure the job ID is valid
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid job ID. Please select a valid job to apply for.";
                return RedirectToAction("Index", "Job");
            }

            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in as a job seeker to apply for jobs.";
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            Console.WriteLine($"JobSeeker ID: {jobSeekerId}");

            // Check if the job seeker has already applied to this job
            if (await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id))
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Details", "Job", new { id });
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the application is valid
            application.JobId = id;
            application.JobSeekerId = jobSeekerId;
            application.Status = "Pending";
            application.ApplicationDate = DateTime.Now;

            // If the user didn't select a CV and didn't check the "No CVs available" option
            if (application.CVId == 0 && NoCVsAvailable != true)
            {
                ModelState.AddModelError("CVId", "Please select a CV or check 'I don't have a CV'.");
            }
            else if (NoCVsAvailable == true)
            {
                application.CVId = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var createdApplication = await _applicationService.CreateApplicationAsync(application);
                    Console.WriteLine($"Application created with ID: {createdApplication.Id}");

                    // Get job seeker name for notification
                    var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
                    string applicantName = jobSeeker?.User.Username ?? "A job seeker";

                    // Send notification to employer about the new application
                    await _notificationService.NotifyNewJobApplicationAsync(job.EmployerId.ToString(), job.Id, applicantName);

                    TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                    return RedirectToAction("Details", "Job", new { id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating application: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while submitting your application. Please try again.");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"Validation errors: {string.Join(", ", errors)}");
            }

            // If we got here, something went wrong
            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.CVs = cvs;
            ViewBag.Job = job;
            return View(application);
        }

        // POST: /Application/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string notes)
        {
            // Ensure the user is an employer
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Ensure the employer owns the job
            var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
            if (application.Job.EmployerId != employerId)
            {
                return Forbid();
            }

            // Update the application status
            var previousStatus = application.Status;
            application.Status = status;

            // Add notes if provided
            if (!string.IsNullOrEmpty(notes))
            {
                application.Notes = notes;
            }

            await _applicationService.UpdateApplicationAsync(application);

            // Send notification to job seeker about the status change
            if (previousStatus != status)
            {
                await _notificationService.NotifyJobApplicationStatusChangedAsync(
                    application.JobSeekerId.ToString(),
                    application.Id,
                    status);
            }

            TempData["SuccessMessage"] = "Application status updated successfully.";
            return RedirectToAction("Details", new { id });
        }

        // GET: /Application/ByJob/5
        public async Task<IActionResult> ByJob(int id)
        {
            // Ensure the user is an employer
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
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

            return View(applications);
        }

        // GET: /Application/ByStatus
        public async Task<IActionResult> ByStatus(string status)
        {
            // Ensure the user is an employer
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
            var applications = await _applicationService.GetApplicationsByStatusAsync(status);

            // Filter applications to only show those for jobs posted by this employer
            var employerApplications = applications.Where(a => a.Job.EmployerId == employerId);

            ViewBag.Status = status;

            return View("EmployerApplications", employerApplications);
        }

        // POST: /Application/UpdateNotes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(int id, string notes)
        {
            Console.WriteLine($"UpdateNotes called with id={id}, notes length={notes?.Length ?? 0}");

            // Ensure the user is an employer
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                Console.WriteLine("UpdateNotes: User is not an employer");
                return Json(new { success = false, message = "You must be logged in as an employer." });
            }

            var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
            Console.WriteLine($"UpdateNotes: EmployerId={employerId}");

            var application = await _applicationService.GetApplicationByIdAsync(id);

            if (application == null)
            {
                Console.WriteLine($"UpdateNotes: Application with id={id} not found");
                return Json(new { success = false, message = "Application not found." });
            }

            // Ensure the application belongs to a job posted by this employer
            if (application.Job.EmployerId != employerId)
            {
                Console.WriteLine($"UpdateNotes: Application belongs to employer {application.Job.EmployerId}, not {employerId}");
                return Json(new { success = false, message = "You don't have permission to update this application." });
            }

            try
            {
                // Update the notes
                application.Notes = notes;
                await _applicationService.UpdateApplicationAsync(application);
                Console.WriteLine("UpdateNotes: Notes updated successfully");

                return Json(new
                {
                    success = true,
                    notes = string.IsNullOrEmpty(notes) ?
                        "<p class=\"text-muted mb-0\">No notes added yet.</p>" :
                        notes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateNotes: Error updating notes - {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating notes." });
            }
        }
    }
}