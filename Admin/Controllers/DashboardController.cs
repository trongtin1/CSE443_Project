using Microsoft.AspNetCore.Mvc;
using Admin.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // For demo purposes, we're creating mock data
            // In a real application, you would fetch this data from a database
            var dashboardViewModel = new DashboardViewModel
            {
                TotalCandidates = 1250,
                ActiveCandidates = 980,
                NewCandidatesToday = 25,
                
                TotalEmployers = 350,
                VerifiedEmployers = 320,
                NewEmployersToday = 8,
                
                TotalJobs = 750,
                ActiveJobs = 520,
                ExpiredJobs = 230,
                NewJobsToday = 15,
                
                TotalApplications = 4500,
                NewApplicationsToday = 120,
                
                CandidateRegistrationsChart = GetLast7DaysData(15, 25),
                JobPostingsChart = GetLast7DaysData(5, 20),
                ApplicationsChart = GetLast7DaysData(50, 150)
            };
            
            return View(dashboardViewModel);
        }
        
        private List<ChartData> GetLast7DaysData(int min, int max)
        {
            var random = new Random();
            var result = new List<ChartData>();
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                result.Add(new ChartData
                {
                    Label = date.ToString("dd/MM"),
                    Value = random.Next(min, max)
                });
            }
            
            return result;
        }
    }
} 