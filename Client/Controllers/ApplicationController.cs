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

        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            IJobSeekerService jobSeekerService,
            ICVService cvService,
            ICandidateService candidateService)
        {
            _applicationService = applicationService;
            _jobService = jobService;
            _jobSeekerService = jobSeekerService;
            _cvService = cvService;
            _candidateService = candidateService;
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

            // Ensure we have a valid JobId
            if (application != null)
            {
                // Always set JobId from route parameter first as it's most reliable
                application.JobId = id;
                Console.WriteLine($"Set JobId from route: {id}");

                // Double-check with form data if available
                var jobIdString = Request.Form["JobId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(jobIdString) && int.TryParse(jobIdString, out int jobId) && jobId > 0)
                {
                    // Log if there's a mismatch
                    if (jobId != id)
                    {
                        Console.WriteLine($"Warning: JobId mismatch. Route: {id}, Form: {jobId}");
                    }
                }
            }

            // Log all form data for debugging
            Console.WriteLine("Form data:");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"  {key} = {Request.Form[key]}");
            }

            // Ensure the user is a job seeker
            if (HttpContext.Session.GetInt32("JobSeekerId") == null)
            {
                Console.WriteLine("No JobSeekerId in session");
                TempData["ErrorMessage"] = "You must be logged in as a job seeker to apply for jobs.";
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId").Value;
            Console.WriteLine($"JobSeeker ID: {jobSeekerId}");

            // Verify the job seeker exists
            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
            {
                Console.WriteLine("Job seeker not found in database");
                TempData["ErrorMessage"] = "Your profile information is incomplete. Please update your profile before applying.";
                return RedirectToAction("Profile", "JobSeeker");
            }

            // Check if the job seeker has already applied to this job
            if (await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id))
            {
                Console.WriteLine("Job seeker already applied");
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Details", "Job", new { id });
            }

            // Verify the job exists before proceeding
            var jobExists = await _jobService.GetJobByIdAsync(id);
            if (jobExists == null)
            {
                // Try to get job again in case of temporary database connection issue
                jobExists = await _jobService.GetJobByIdAsync(id);
                if (jobExists == null)
                {
                    Console.WriteLine($"Job with ID {id} not found in database");
                    TempData["ErrorMessage"] = "The job you're trying to apply for doesn't exist or has been removed.";
                    return RedirectToAction("Index", "Job");
                }
            }

            // Make sure we have the correct JobId
            application.JobId = id;
            application.JobSeekerId = jobSeekerId;
            application.ApplicationDate = DateTime.Now;
            application.Status = "Pending";

            // Don't set ID as it's an identity column
            application.Id = 0;

            // Parse CVId from form if it's not set
            if (application.CVId <= 0 || application.CVId == null)
            {
                var cvIdString = Request.Form["CVId"].FirstOrDefault();
                if (!string.IsNullOrEmpty(cvIdString) && int.TryParse(cvIdString, out int cvId))
                {
                    application.CVId = cvId;
                    Console.WriteLine($"Parsed CVId from form: {cvId}");
                }
            }

            // Check if CV is required but not provided
            if (NoCVsAvailable != true && (application.CVId <= 0 || application.CVId == null))
            {
                Console.WriteLine("CV required but not provided");
                ModelState.AddModelError("CVId", "Please select a CV or create one first.");
            }

            if (string.IsNullOrWhiteSpace(application.CoverLetter))
            {
                Console.WriteLine("Cover letter is empty");
                ModelState.AddModelError("CoverLetter", "Cover letter is required.");
            }

            // Remove validation for navigation properties
            ModelState.Remove("Job");
            ModelState.Remove("JobSeeker");
            ModelState.Remove("CV");

            Console.WriteLine($"ModelState valid: {ModelState.IsValid}");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Creating application in database");
                    Console.WriteLine($"Application data: JobId={application.JobId}, JobSeekerId={application.JobSeekerId}, CVId={application.CVId}, CoverLetter={application.CoverLetter?.Substring(0, Math.Min(20, application.CoverLetter?.Length ?? 0))}...");

                    var createdApplication = await _applicationService.CreateApplicationAsync(application);
                    Console.WriteLine($"Application created with ID: {createdApplication.Id}");

                    TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                    return RedirectToAction("Details", "Job", new { id });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error submitting application: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");

                        // Check for specific database errors
                        if (ex.InnerException.Message.Contains("FK_Applications_Jobs_JobId"))
                        {
                            // Try to update the job reference and save again
                            try
                            {
                                Console.WriteLine("Attempting to fix job reference and retry...");
                                var currentJob = await _jobService.GetJobByIdAsync(id);
                                if (currentJob != null)
                                {
                                    // Job exists, try to save application again
                                    application.JobId = currentJob.Id;
                                    var createdApplication = await _applicationService.CreateApplicationAsync(application);
                                    Console.WriteLine($"Application created with ID: {createdApplication.Id} after retry");
                                    TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                                    return RedirectToAction("Details", "Job", new { id });
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "The job you're trying to apply for no longer exists.";
                                    ModelState.AddModelError("", "The job you're trying to apply for no longer exists.");
                                    return RedirectToAction("Index", "Job");
                                }
                            }
                            catch (Exception retryEx)
                            {
                                Console.WriteLine($"Retry failed: {retryEx.Message}");
                                TempData["ErrorMessage"] = "The job you're trying to apply for no longer exists.";
                                ModelState.AddModelError("", "The job you're trying to apply for no longer exists.");
                                return RedirectToAction("Index", "Job");
                            }
                        }
                        else if (ex.InnerException.Message.Contains("FK_Applications_JobSeekers_JobSeekerId"))
                        {
                            TempData["ErrorMessage"] = "There was an issue with your profile. Please update your profile before applying.";
                            ModelState.AddModelError("", "There was an issue with your profile. Please update your profile before applying.");
                            return RedirectToAction("Profile", "JobSeeker");
                        }
                    }

                    // Set a more user-friendly error message for general errors
                    TempData["ErrorMessage"] = "There was an error submitting your application. Please try again.";
                    ModelState.AddModelError("", "There was an error submitting your application. Please try again.");
                }
            }

            // If we got this far, something failed; redisplay form
            var job = await _jobService.GetJobByIdAsync(id);
            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.CVs = cvs;
            ViewBag.Job = job;

            return View(application);
        }

        // POST: /Application/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status, string notes)
        {
            // Ensure the user is an employer
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = HttpContext.Session.GetInt32("EmployerId").Value;
            var application = await _applicationService.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Ensure the application belongs to a job posted by this employer
            if (application.Job.EmployerId != employerId)
            {
                return Forbid();
            }

            await _applicationService.UpdateApplicationStatusAsync(id, status, notes);

            // If the status is set to "Shortlisted", create a candidate entry
            if (status == "Shortlisted")
            {
                var candidate = new Candidate
                {
                    ApplicationId = application.Id,
                    JobId = application.JobId,
                    JobSeekerId = application.JobSeekerId,
                    Status = "Shortlisted",
                    ShortlistedDate = DateTime.Now
                };

                await _candidateService.CreateCandidateAsync(candidate);
            }

            return RedirectToAction(nameof(Details), new { id });
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