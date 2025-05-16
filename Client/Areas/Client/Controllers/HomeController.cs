using Microsoft.AspNetCore.Mvc;

namespace CSE443_Project.Areas.Client.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult CreateCV()
        {
            return View();
        }
        
        public IActionResult ApplyJobs()
        {
            return View();
        }
        
        public IActionResult SavedJobs()
        {
            return View();
        }
        
        public IActionResult MyApplications()
        {
            return View();
        }
    }
}
