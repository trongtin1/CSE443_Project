using Microsoft.AspNetCore.Mvc;

namespace CSE443_Project.Areas.Client.Controllers
{
    [Area("Client")]
    public class RecruiterController : Controller
    {
        public IActionResult Dashboard()
        {
            // In a real application, you would check if the user is authenticated
            // and retrieve the recruiter's information from the database
            return View();
        }
        
        public IActionResult CreateJob()
        {
            return View();
        }
        
        public IActionResult ManageJobs()
        {
            return View();
        }
        
        public IActionResult Applications()
        {
            return View();
        }
        
        public IActionResult CompanyProfile()
        {
            return View();
        }
        
        public IActionResult Reports()
        {
            return View();
        }
    }
} 