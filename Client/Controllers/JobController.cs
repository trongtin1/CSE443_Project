using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CSE443_Project.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobCategoryService _categoryService;
        private readonly IEmployerService _employerService;
        private readonly ISaveJobService _saveJobService;
        private readonly IApplicationService _applicationService;

        public JobController(
            IJobService jobService,
            IJobCategoryService categoryService,
            IEmployerService employerService,
            ISaveJobService saveJobService,
            IApplicationService applicationService)
        {
            _jobService = jobService;
            _categoryService = categoryService;
            _employerService = employerService;
            _saveJobService = saveJobService;
            _applicationService = applicationService;
        }

        // GET: /Job
        public async Task<IActionResult> Index(string searchTerm = null)
        {
            var jobs = string.IsNullOrEmpty(searchTerm)
                ? await _jobService.GetActiveJobsAsync()
                : await _jobService.SearchJobsAsync(searchTerm);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();

            return View(jobs);
        }

        // GET: /Job/Filter
        public async Task<IActionResult> Filter(string location, string jobType, decimal? minSalary, decimal? maxSalary, bool? isRemote, int? categoryId)
        {
            var jobs = await _jobService.FilterJobsAsync(location, jobType, minSalary, maxSalary, isRemote, categoryId);

            ViewBag.Location = location;
            ViewBag.JobType = jobType;
            ViewBag.MinSalary = minSalary;
            ViewBag.MaxSalary = maxSalary;
            ViewBag.IsRemote = isRemote;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();

            return View("Index", jobs);
        }

        // GET: /Job/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            ViewBag.ApplicationCount = await _applicationService.GetApplicationCountByJobIdAsync(id);

            // Check if the current user has saved this job
            if (TempData.ContainsKey("JobSeekerId"))
            {
                var jobSeekerId = (int)TempData["JobSeekerId"];
                TempData.Keep("JobSeekerId"); // Keep for the next request

                ViewBag.HasSaved = await _saveJobService.HasJobSeekerSavedJobAsync(jobSeekerId, id);
                ViewBag.HasApplied = await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId, id);
            }

            return View(job);
        }

        // GET: /Job/Create
        public async Task<IActionResult> Create()
        {
            // Ensure the user is an employer
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();
            return View();
        }

        // POST: /Job/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                job.EmployerId = (int)TempData["EmployerId"];
                TempData.Keep("EmployerId");

                job.CreatedAt = DateTime.Now;
                job.IsActive = true;

                await _jobService.CreateJobAsync(job);
                return RedirectToAction(nameof(Details), new { id = job.Id });
            }

            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();
            return View(job);
        }

        // GET: /Job/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != (int)TempData["EmployerId"])
            {
                return Forbid();
            }

            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();
            return View(job);
        }

        // POST: /Job/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Job job)
        {
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            if (id != job.Id)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != (int)TempData["EmployerId"])
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                await _jobService.UpdateJobAsync(job);
                return RedirectToAction(nameof(Details), new { id = job.Id });
            }

            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();
            return View(job);
        }

        // POST: /Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != (int)TempData["EmployerId"])
            {
                return Forbid();
            }

            await _jobService.DeleteJobAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Job/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            if (!TempData.ContainsKey("EmployerId"))
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != (int)TempData["EmployerId"])
            {
                return Forbid();
            }

            await _jobService.DeactivateJobAsync(id);
            return RedirectToAction(nameof(Details), new { id = job.Id });
        }

        // GET: /Job/ByCategory/5
        public async Task<IActionResult> ByCategory(int id)
        {
            var category = await _categoryService.GetJobCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var jobs = await _jobService.GetJobsByCategoryIdAsync(id);

            ViewBag.CategoryName = category.Name;
            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();

            return View("Index", jobs);
        }

        // GET: /Job/ByEmployer/5
        public async Task<IActionResult> ByEmployer(int id)
        {
            var employer = await _employerService.GetEmployerByIdAsync(id);
            if (employer == null)
            {
                return NotFound();
            }

            var jobs = await _jobService.GetJobsByEmployerIdAsync(id);

            ViewBag.EmployerName = employer.CompanyName;
            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();

            return View("Index", jobs);
        }
    }
}