using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSE443_Project.Data;
using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CVsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CVsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/CVs
        public IActionResult Index()
        {
            try
            {
                var cvs = _context.CVs
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .ToList();
                return View(cvs);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error fetching CVs: {ex.Message}");
                
                // Check if error is related to missing table
                if (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message.Contains("Invalid object name") == true)
                {
                    ViewBag.ErrorMessage = "The database schema is not properly set up. Please run migrations to create the required tables.";
                    return View(new List<CV>());
                }
                
                // Return empty list with error message for other errors
                ViewBag.ErrorMessage = "An error occurred while fetching CVs. Please try again later.";
                return View(new List<CV>());
            }
        }

        // GET: Admin/CVs/Create
        public IActionResult Create()
        {
            try
            {
                // Get job seekers for dropdown
                var jobSeekers = _context.JobSeekers.Include(js => js.User).ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username");
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading job seekers: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/CVs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CV cv, IFormFile cvFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (cvFile != null && cvFile.Length > 0)
                    {
                        // Create CVs directory if it doesn't exist
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "cvs");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate unique filename
                        string uniqueFileName = $"{DateTime.Now.Ticks}_{Path.GetFileName(cvFile.FileName)}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await cvFile.CopyToAsync(fileStream);
                        }

                        // Save file path in database
                        cv.FilePath = $"/uploads/cvs/{uniqueFileName}";
                        cv.CreatedAt = DateTime.Now;

                        _context.CVs.Add(cv);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("cvFile", "Please select a file to upload.");
                    }
                }

                // If we got to here, something went wrong, redisplay form
                var jobSeekers = _context.JobSeekers.Include(js => js.User).ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username", cv.JobSeekerId);
                return View(cv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error uploading CV: " + ex.Message;
                var jobSeekers = _context.JobSeekers.Include(js => js.User).ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username", cv.JobSeekerId);
                return View(cv);
            }
        }

        // GET: Admin/CVs/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var cv = _context.CVs
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .FirstOrDefault(c => c.Id == id);
                    
                if (cv == null) return NotFound();
                return View(cv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the CV details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/CVs/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                var cv = _context.CVs
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .FirstOrDefault(c => c.Id == id);
                    
                if (cv == null) return NotFound();
                return View(cv);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the CV to delete.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/CVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cv = await _context.CVs.FindAsync(id);
                if (cv == null) return NotFound();

                // Delete file if it exists
                if (!string.IsNullOrEmpty(cv.FilePath))
                {
                    string fullPath = Path.Combine(_hostingEnvironment.WebRootPath, cv.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.CVs.Remove(cv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the CV.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/CVs/Download/5
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var cv = await _context.CVs.FindAsync(id);
                if (cv == null) return NotFound();

                if (string.IsNullOrEmpty(cv.FilePath))
                {
                    ViewBag.ErrorMessage = "CV file not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get file path
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, cv.FilePath.TrimStart('/'));
                
                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    ViewBag.ErrorMessage = "CV file not found on server.";
                    return RedirectToAction(nameof(Index));
                }

                // Determine content type
                string contentType = "application/octet-stream";
                if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/pdf";
                }
                else if (filePath.EndsWith(".doc", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/msword";
                }
                else if (filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                }

                // Return file
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error downloading CV: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/CVs/Preview/5
        public async Task<IActionResult> Preview(int id)
        {
            try
            {
                var cv = await _context.CVs.FindAsync(id);
                if (cv == null) return NotFound();

                if (string.IsNullOrEmpty(cv.FilePath))
                {
                    ViewBag.ErrorMessage = "CV file not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get file path
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, cv.FilePath.TrimStart('/'));
                
                // Check if file exists
                if (!System.IO.File.Exists(filePath))
                {
                    ViewBag.ErrorMessage = "CV file not found on server.";
                    return RedirectToAction(nameof(Index));
                }

                // Only preview PDF files directly
                if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(fileBytes, "application/pdf");
                }
                
                // For other file types, redirect to download
                return RedirectToAction(nameof(Download), new { id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error previewing CV: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/CVs/SetDefault/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            try
            {
                var cv = await _context.CVs.FindAsync(id);
                if (cv == null) return NotFound();

                // Get all CVs for this job seeker
                var jobSeekerCVs = await _context.CVs
                    .Where(c => c.JobSeekerId == cv.JobSeekerId)
                    .ToListAsync();

                // Set all to non-default
                foreach (var jobSeekerCV in jobSeekerCVs)
                {
                    jobSeekerCV.IsDefault = false;
                }

                // Set this one as default
                cv.IsDefault = true;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error setting CV as default: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}