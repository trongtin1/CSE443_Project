using Microsoft.AspNetCore.Mvc;

namespace CSE443_Project.Areas.Client.Controllers
{
    [Area("Client")]
    public class CandidateController : Controller
    {
        public IActionResult Dashboard()
        {
            // All candidate views now have protection in the _CandidateLayout.cshtml
            // which checks localStorage and redirects if not authenticated
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
        
        public IActionResult Profile()
        {
            return View();
        }
    }
} 