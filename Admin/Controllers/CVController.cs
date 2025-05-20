using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin.Data;
using Admin.Models;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class CVController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CVController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CV
        public async Task<IActionResult> Index()
        {
            var cvs = await _context.CVs
                .Include(c => c.User)
                .ToListAsync();
            return View(cvs);
        }

        // GET: CV/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.CVs
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cv == null)
            {
                return NotFound();
            }

            return View(cv);
        }

        // GET: CV/Create
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // POST: CV/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Content")] CV cv)
        {
            if (ModelState.IsValid)
            {
                cv.CreatedAt = DateTime.Now;
                _context.Add(cv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.ToList();
            return View(cv);
        }

        // GET: CV/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.CVs.FindAsync(id);
            if (cv == null)
            {
                return NotFound();
            }
            ViewBag.Users = _context.Users.ToList();
            return View(cv);
        }

        // POST: CV/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Content,CreatedAt")] CV cv)
        {
            if (id != cv.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cv);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CVExists(cv.Id))
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
            ViewBag.Users = _context.Users.ToList();
            return View(cv);
        }

        // GET: CV/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cv = await _context.CVs
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cv == null)
            {
                return NotFound();
            }

            return View(cv);
        }

        // POST: CV/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cv = await _context.CVs.FindAsync(id);
            if (cv != null)
            {
                _context.CVs.Remove(cv);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CVExists(int id)
        {
            return _context.CVs.Any(e => e.Id == id);
        }
    }
} 