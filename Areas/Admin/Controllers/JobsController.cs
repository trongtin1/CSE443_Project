using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var jobs = _context.Jobs.ToList();
            return View(jobs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        public IActionResult Edit(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Jobs.Update(job);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        public IActionResult Delete(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return NotFound();
            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return NotFound();

            _context.Jobs.Remove(job);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return NotFound();
            return View(job);
        }
    }
}
