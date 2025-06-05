using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Data;
using CSE443_Project.Models.ViewModels;

namespace CSE443_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
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
                TotalRevenue = _context.Transactions.Sum(t => t.Amount)
            };

            return View(model);
        }
    }
}
