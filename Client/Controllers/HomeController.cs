using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace CSE443_Project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJobService _jobService;
    private readonly IJobCategoryService _categoryService;
    private readonly IEmployerService _employerService;

    public HomeController(
        ILogger<HomeController> logger,
        IJobService jobService,
        IJobCategoryService categoryService,
        IEmployerService employerService)
    {
        _logger = logger;
        _jobService = jobService;
        _categoryService = categoryService;
        _employerService = employerService;
    }

    public async Task<IActionResult> Index()
    {
        // Get latest active jobs for the homepage
        var latestJobs = await _jobService.GetActiveJobsAsync();
        ViewBag.LatestJobs = latestJobs.Take(6).ToList();

        // Get job categories with job count
        var categories = await _categoryService.GetAllJobCategoriesAsync();
        ViewBag.Categories = categories;

        foreach (var category in categories)
        {
            ViewData[$"JobCount_{category.Id}"] = await _categoryService.GetJobCountByCategoryIdAsync(category.Id);
        }

        // Get featured employers
        var employers = await _employerService.GetAllEmployersAsync();
        ViewBag.FeaturedEmployers = employers.Take(4).ToList();

        return View();
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await _categoryService.GetAllJobCategoriesAsync();

        foreach (var category in categories)
        {
            ViewData[$"JobCount_{category.Id}"] = await _categoryService.GetJobCountByCategoryIdAsync(category.Id);
        }

        return View(categories);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SubmitContact(string name, string email, string phone, string subject, string message)
    {
        // TODO: Implement actual contact form processing logic
        // This would typically involve sending an email or saving to database
        
        // Log the contact form submission
        _logger.LogInformation($"Contact form submitted by {name} ({email}) regarding {subject}");
        
        // Add success message
        TempData["SuccessMessage"] = "Thank you for your message! We'll get back to you soon.";
        
        // Redirect back to contact page
        return RedirectToAction(nameof(Contact));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
