using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IJobCategoryService _categoryService;
        private readonly IJobService _jobService;

        public CategoryController(
            IJobCategoryService categoryService,
            IJobService jobService)
        {
            _categoryService = categoryService;
            _jobService = jobService;
        }

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllJobCategoriesAsync();

            foreach (var category in categories)
            {
                ViewData[$"JobCount_{category.Id}"] = await _categoryService.GetJobCountByCategoryIdAsync(category.Id);
            }

            return View(categories);
        }

        // GET: /Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetJobCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var jobs = await _jobService.GetJobsByCategoryIdAsync(id);
            ViewBag.Jobs = jobs;
            ViewBag.JobCount = jobs.Count();

            return View(category);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobCategory category)
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                await _categoryService.CreateJobCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            var category = await _categoryService.GetJobCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobCategory category)
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateJobCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            var category = await _categoryService.GetJobCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if the user is an admin
            if (!TempData.ContainsKey("UserId"))
            {
                return RedirectToAction("Login", "User");
            }

            await _categoryService.DeleteJobCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}