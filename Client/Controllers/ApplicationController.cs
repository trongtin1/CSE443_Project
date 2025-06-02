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

        // GET: /Application
        public async Task<IActionResult> Index()
        {
            // Check if user is a job seeker or employer
            if (TempData.ContainsKey("JobSeekerId"))
            {
                var jobSeekerId = (int)TempData["JobSeekerId"];
                TempData.Keep("JobSeekerId");

                var applications = await _applicationService.GetApplicationsByJobSeekerIdAsync(jobSeekerId);
                return View("JobSeekerApplications", applications);
            }
            else if (TempData.ContainsKey("EmployerId"))
            {
                var employerId = (int)TempData["EmployerId"];
                TempData.Keep("EmployerId");

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
            if (TempData.ContainsKey("JobSeekerId"))
            {
                var jobSeekerId = (int)TempData["JobSeekerId"];
                TempData.Keep("JobSeekerId");

                if (application.JobSeekerId != jobSeekerId)
                {
                    return Forbid();
                }
            }
            else if (TempData.ContainsKey("EmployerId"))
            {
                var employerId = (int)TempData["EmployerId"];
                TempData.Keep("EmployerId");

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
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            // Check if the job seeker has already applied to this job
            if (await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id))
            {
                return RedirectToAction("Details", "Job", new { id });
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Get the job seeker's CVs for selection
            var cvs = await _cvService.GetCVsByJobSeekerIdAsync(jobSeekerId);
            ViewBag.CVs = cvs;
            ViewBag.Job = job;

            return View();
        }

        // POST: /Application/Apply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id, Application application)
        {
            // Ensure the user is a job seeker
            if (!TempData.ContainsKey("JobSeekerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var jobSeekerId = (int)TempData["JobSeekerId"];
            TempData.Keep("JobSeekerId");

            // Check if the job seeker has already applied to this job
            if (await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id))
            {
                return RedirectToAction("Details", "Job", new { id });
            }

            if (ModelState.IsValid)
            {
                application.JobId = id;
                application.JobSeekerId = jobSeekerId;
                application.ApplicationDate = DateTime.Now;
                application.Status = "Pending";

                await _applicationService.CreateApplicationAsync(application);

                return RedirectToAction("Details", "Job", new { id });
            }

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
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

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
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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

            return View(applications);
        }

        // GET: /Application/ByStatus
        public async Task<IActionResult> ByStatus(string status)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

            var applications = await _applicationService.GetApplicationsByStatusAsync(status);

            // Filter applications to only show those for jobs posted by this employer
            var employerApplications = applications.Where(a => a.Job.EmployerId == employerId);

            ViewBag.Status = status;

            return View("EmployerApplications", employerApplications);
        }
    }
}