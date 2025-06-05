using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var apps = _context.Applications.ToList();
            return View(apps);
        }

        public IActionResult Details(int id)
        {
            var app = _context.Applications.Find(id);
            if (app == null) return NotFound();
            return View(app);
        }

        public IActionResult Delete(int id)
        {
            var app = _context.Applications.Find(id);
            if (app == null) return NotFound();
            return View(app);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var app = _context.Applications.Find(id);
            if (app == null) return NotFound();

            _context.Applications.Remove(app);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
