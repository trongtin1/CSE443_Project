using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSE443_Project.Data;
using CSE443_Project.Models;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Candidates.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.JobSeeker != null &&
                    c.JobSeeker.User != null && (
                        c.JobSeeker.User.Username.Contains(search) ||
                        c.JobSeeker.User.Email.Contains(search)
                    )
                );
            }
            return View(query.ToList());
        }

        public IActionResult Details(int id)
        {
            var candidate = _context.Candidates
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    Candidate = c,
                    JobSeeker = c.JobSeeker,
                    Job = c.Job,
                    Application = c.Application
                })
                .FirstOrDefault();

            if (candidate == null) return NotFound();
            return View(candidate.Candidate);
        }

        public IActionResult Create()
        {
            // Optionally load related data for dropdowns if needed
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Candidate model)
        {
            if (ModelState.IsValid)
            {
                model.ShortlistedDate = DateTime.Now;
                _context.Candidates.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Candidate model)
        {
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _context.Candidates.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}