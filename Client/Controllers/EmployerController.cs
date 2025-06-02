using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
        }

        // GET: /Employer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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
        }

        // GET: /Employer/Profile
        public async Task<IActionResult> Profile()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

            var employer = await _employerService.GetEmployerByIdAsync(employerId);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: /Employer/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Employer employer)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

            if (employer.Id != employerId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                await _employerService.UpdateEmployerAsync(employer);
                return RedirectToAction(nameof(Profile));
            }

            return View("Profile", employer);
        }

        // GET: /Employer/Jobs
        public async Task<IActionResult> Jobs()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

            var jobs = await _jobService.GetJobsByEmployerIdAsync(employerId);
            return View(jobs);
        }

        // GET: /Employer/Applications
        public async Task<IActionResult> Applications()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
            TempData.Keep("EmployerId");

            var applications = await _applicationService.GetApplicationsByEmployerIdAsync(employerId);
            return View(applications);
        }

        // GET: /Employer/ApplicationsByStatus
        public async Task<IActionResult> ApplicationsByStatus(string status)
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

            return View("Applications", employerApplications);
        }

        // GET: /Employer/ApplicationsByJob/5
        public async Task<IActionResult> ApplicationsByJob(int id)
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

            return View("Applications", applications);
        }

        // GET: /Employer/Candidates
        public async Task<IActionResult> Candidates()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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

            var candidates = await _candidateService.GetCandidatesByJobIdAsync(id);

            ViewBag.Job = job;

            return View("Candidates", candidates);
        }

        // GET: /Employer/CandidatesByStatus
        public async Task<IActionResult> CandidatesByStatus(string status)
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var employerId = (int)TempData["EmployerId"];
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
    }
}