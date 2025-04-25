using Microsoft.AspNetCore.Mvc;

namespace CSE443_Project.Areas.Client.Controllers
{
    [Area("Client")]
    public class CandidateController : Controller
    {
        public IActionResult Dashboard()
        {
            // In a real application, you would check if the user is authenticated
            // and retrieve the candidate's information from the database
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