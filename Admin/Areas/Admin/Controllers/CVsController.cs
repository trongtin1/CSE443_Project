using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        public IActionResult Index(string searchTerm = null)
        {
            var cvsQuery = _context.CVs.Include(c => c.User).AsQueryable();
            
            // Áp dụng tìm kiếm nếu có searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                cvsQuery = cvsQuery.Where(c => 
                    c.User.name.ToLower().Contains(searchTerm) ||
                    c.User.email.ToLower().Contains(searchTerm) ||
                    c.Content.ToLower().Contains(searchTerm)
                );
            }
            
            var cvs = cvsQuery.ToList();
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
        
        // GET: Admin/CVs/Download/5
        public IActionResult Download(int id)
        {
            var cv = _context.CVs.Find(id);
            if (cv == null) return NotFound();
            
            // Giả sử CV.Content chứa đường dẫn tới file
            string filePath = cv.Content;
            string fileName = $"CV_{cv.UserId}_{id}.pdf";
            
            // Trả về file để tải xuống
            if (System.IO.File.Exists(filePath))
            {
                return PhysicalFile(filePath, "application/pdf", fileName);
            }
            
            return NotFound("File không tồn tại");
        }
    }
}
