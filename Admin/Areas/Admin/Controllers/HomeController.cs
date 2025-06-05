using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Models;
using Microsoft.AspNetCore.Authorization;
using CSE443_Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Get statistics for dashboard
                ViewBag.TotalCandidates = _context.Candidates.Count();
                ViewBag.TotalUsers = _context.Users.Count();
                ViewBag.TotalCVs = _context.CVs.Count();
                ViewBag.ActiveJobs = _context.Jobs.Count(j => j.IsActive);

                // Get recent CVs with related data
                var recentCVs = _context.CVs
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToList();

                return View(recentCVs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new List<CV>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}