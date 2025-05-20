using Microsoft.AspNetCore.Mvc;
using Admin.Data;
using Admin.Models.ViewModels;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalJobs = _context.Jobs.Count(),
                TotalApplications = _context.Applications.Count(),
                TotalCVs = _context.CVs.Count()
            };

            return View(model);
        }
    }
}
