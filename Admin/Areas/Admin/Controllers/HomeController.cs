using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Admin.Data;
using Microsoft.EntityFrameworkCore;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalJobs = await _context.Jobs.CountAsync();
            ViewBag.TotalApplications = await _context.Applications.CountAsync();
            ViewBag.TotalCVs = await _context.CVs.CountAsync();

            return View();
        }
    }
} 