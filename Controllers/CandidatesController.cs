using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSE443_Project.Models;
using CSE443_Project.Data;

namespace CSE443_Project.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var candidates = _context.Candidates
                .Include(c => c.Job)
                .Include(c => c.JobSeeker)
                .ThenInclude(js => js.User)
                .ToList();
            return View(candidates);
        }

        public IActionResult Create()
        {
            var applications = _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                .ThenInclude(js => js.User)
                .ToList();
            ViewBag.Applications = applications;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                var application = _context.Applications
                    .Include(a => a.Job)
                    .Include(a => a.JobSeeker)
                    .FirstOrDefault(a => a.Id == candidate.ApplicationId);

                if (application == null)
                {
                    ModelState.AddModelError("ApplicationId", "Invalid application selected.");
                    ViewBag.Applications = _context.Applications
                        .Include(a => a.Job)
                        .Include(a => a.JobSeeker)
                        .ThenInclude(js => js.User)
                        .ToList();
                    return View(candidate);
                }

                candidate.JobId = application.JobId;
                candidate.JobSeekerId = application.JobSeekerId;

                _context.Candidates.Add(candidate);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Applications = _context.Applications
                .Include(a => a.Job)
                .Include(a => a.JobSeeker)
                .ThenInclude(js => js.User)
                .ToList();
            return View(candidate);
        }

        public IActionResult Edit(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                _context.Candidates.Update(candidate);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        public IActionResult Delete(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var candidate = _context.Candidates
                .Include(c => c.Job)
                .Include(c => c.JobSeeker)
                .ThenInclude(js => js.User)
                .FirstOrDefault(c => c.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }
    }
}