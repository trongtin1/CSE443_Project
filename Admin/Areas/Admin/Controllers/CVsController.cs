using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CVsController : Controller
    {
        private readonly AppDbContext _context;

        public CVsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CVs
        public IActionResult Index()
        {
            var cvs = _context.CVs.Include(c => c.User).ToList();
            return View(cvs);
        }

        // GET: Admin/CVs/Details/5
        public IActionResult Details(int id)
        {
            var cv = _context.CVs.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            if (cv == null) return NotFound();
            return View(cv);
        }

        // GET: Admin/CVs/Delete/5
        public IActionResult Delete(int id)
        {
            var cv = _context.CVs.Include(c => c.User).FirstOrDefault(c => c.Id == id);
            if (cv == null) return NotFound();
            return View(cv);
        }

        // POST: Admin/CVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var cv = _context.CVs.Find(id);
            if (cv == null) return NotFound();

            _context.CVs.Remove(cv);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
