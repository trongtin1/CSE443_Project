using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            var jobSeekerId = HttpContext.Session.GetInt32("JobSeekerId");
            if (jobSeekerId != null)
            {
                ViewBag.HasSaved = await _saveJobService.HasJobSeekerSavedJobAsync(jobSeekerId.Value, id);
                ViewBag.HasApplied = await _applicationService.HasJobSeekerAppliedToJobAsync(jobSeekerId.Value, id);
            }

            return View(job);
        }

        // GET: /Job/Create
        public async Task<IActionResult> Create()
        {
            // Ensure the user is an employer
            var employerId = HttpContext.Session.GetInt32("EmployerId");
            if (employerId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in as an employer to post jobs.";
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
            var employerId = HttpContext.Session.GetInt32("EmployerId");
            if (employerId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in as an employer to post jobs.";
                return RedirectToAction("Login", "User");
            }

            try
            {
                // Log all form data for debugging
                var formData = Request.Form;
                foreach (var key in formData.Keys)
                {
                    Console.WriteLine($"Form data: {key} = {formData[key]}");
                }

                // Remove validation errors for navigation properties
                ModelState.Remove("Employer");
                ModelState.Remove("Category");

                if (ModelState.IsValid)
                {
                    job.EmployerId = employerId.Value;
                    job.CreatedAt = DateTime.Now;
                    job.IsActive = true;

                    Console.WriteLine($"Creating job: {job.JobTitle}, EmployerId: {job.EmployerId}");

                    try
                    {
                        var createdJob = await _jobService.CreateJobAsync(job);
                        Console.WriteLine($"Job created with ID: {createdJob.Id}");

                        TempData["SuccessMessage"] = "Job posted successfully!";
                        return RedirectToAction(nameof(Details), new { id = createdJob.Id });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Database save error: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                        ModelState.AddModelError("", "Failed to save job to database. Please try again.");
                        TempData["ErrorMessage"] = "Database error: " + ex.Message;
                    }
                }
                else
                {
                    // Capture model validation errors for debugging
                    var errors = ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation error for {error.Key}: {string.Join(", ", error.Value)}");
                    }

                    ViewBag.ModelErrors = errors;
                    TempData["ErrorMessage"] = "Please fix the validation errors below.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                TempData["ErrorMessage"] = $"An error occurred while posting the job: {ex.Message}";
            }

            // Always reload categories and return the view with the job when there's an error
            ViewBag.Categories = await _categoryService.GetAllJobCategoriesAsync();
            return View(job);
        }

        // GET: /Job/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != HttpContext.Session.GetInt32("EmployerId").Value)
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
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (id != job.Id)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != HttpContext.Session.GetInt32("EmployerId").Value)
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
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != HttpContext.Session.GetInt32("EmployerId").Value)
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
            if (HttpContext.Session.GetInt32("EmployerId") == null)
            {
                return RedirectToAction("Login", "User");
            }

            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            // Ensure the job belongs to the current employer
            if (job.EmployerId != HttpContext.Session.GetInt32("EmployerId").Value)
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