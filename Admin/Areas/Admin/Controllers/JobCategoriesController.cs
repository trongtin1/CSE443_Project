using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class JobCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/JobCategories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.JobCategories
                .Include(c => c.Jobs)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/JobCategories/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var jobCategory = await _context.JobCategories
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (jobCategory == null)
            {
                return NotFound();
            }

            return View(jobCategory);
        }

        // GET: Admin/JobCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/JobCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobCategory jobCategory)
        {
            if (ModelState.IsValid)
            {
                _context.JobCategories.Add(jobCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobCategory);
        }

        // GET: Admin/JobCategories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var jobCategory = await _context.JobCategories.FindAsync(id);
            if (jobCategory == null)
            {
                return NotFound();
            }
            return View(jobCategory);
        }

        // POST: Admin/JobCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobCategory jobCategory)
        {
            if (id != jobCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobCategoryExists(jobCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jobCategory);
        }

        // GET: Admin/JobCategories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var jobCategory = await _context.JobCategories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (jobCategory == null)
            {
                return NotFound();
            }

            return View(jobCategory);
        }

        // POST: Admin/JobCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobCategory = await _context.JobCategories.FindAsync(id);
            if (jobCategory == null)
            {
                return NotFound();
            }

            _context.JobCategories.Remove(jobCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobCategoryExists(int id)
        {
            return _context.JobCategories.Any(e => e.Id == id);
        }
    }
}
