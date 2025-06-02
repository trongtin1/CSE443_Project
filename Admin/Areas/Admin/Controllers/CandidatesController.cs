using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSE443_Project.Data;
using CSE443_Project.Models;
using Admin.Models;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : Controller
    {
        private readonly AppDbContext _context;

        public CandidatesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Candidates.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.FullName.Contains(search) || c.Email.Contains(search));
            return View(query.ToList());
        }

        public IActionResult Details(int id)
        {
            var candidate = _context.Candidates.Find(id);
            if (candidate == null) return NotFound();
            return View(candidate);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Candidate model)
        {
            if (ModelState.IsValid)
            {
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