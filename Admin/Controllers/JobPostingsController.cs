using Microsoft.AspNetCore.Mvc;
using Admin.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Admin.Controllers
{
    [Authorize]
    public class JobPostingsController : Controller
    {
        private readonly ILogger<JobPostingsController> _logger;

        public JobPostingsController(ILogger<JobPostingsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // For demo purposes, creating mock job postings
            // In a real application, you would fetch this data from a database
            var jobPostings = GetMockJobPostings();
            return View(jobPostings);
        }

        public IActionResult Details(int id)
        {
            // In a real application, you would fetch the job posting from a database
            var jobPosting = GetMockJobPostings().FirstOrDefault(j => j.Id == id);
            
            if (jobPosting == null)
            {
                return NotFound();
            }
            
            return View(jobPosting);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var jobPosting = GetMockJobPostings().FirstOrDefault(j => j.Id == id);
            
            if (jobPosting == null)
            {
                return NotFound();
            }
            
            return View(jobPosting);
        }

        [HttpPost]
        public IActionResult Edit(JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would update the job posting in a database
                TempData["SuccessMessage"] = "Job posting updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var jobPosting = GetMockJobPostings().FirstOrDefault(j => j.Id == id);
            
            if (jobPosting == null)
            {
                return NotFound();
            }
            
            return View(jobPosting);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // In a real application, you would delete the job posting from a database
            TempData["SuccessMessage"] = "Job posting deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            // In a real application, you would toggle the active status in a database
            TempData["SuccessMessage"] = "Job posting status updated!";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to generate mock job postings for demo
        private List<JobPosting> GetMockJobPostings()
        {
            var jobPostings = new List<JobPosting>();
            var employers = GenerateMockEmployers();
            
            for (int i = 1; i <= 30; i++)
            {
                var employerId = (i % employers.Count) + 1;
                var employer = employers.FirstOrDefault(e => e.Id == employerId);
                
                jobPostings.Add(new JobPosting
                {
                    Id = i,
                    EmployerId = employerId,
                    Employer = employer,
                    Title = $"Job Position {i}",
                    Description = $"Description for Job Position {i}. We are looking for a talented professional to join our team.",
                    Location = i % 4 == 0 ? "Remote" : (i % 4 == 1 ? "New York" : (i % 4 == 2 ? "San Francisco" : "Chicago")),
                    JobType = i % 3 == 0 ? "Full-time" : (i % 3 == 1 ? "Part-time" : "Contract"),
                    ExperienceLevel = i % 4 == 0 ? "Entry Level" : (i % 4 == 1 ? "Mid Level" : (i % 4 == 2 ? "Senior Level" : "Executive")),
                    EducationLevel = i % 3 == 0 ? "Bachelor's Degree" : (i % 3 == 1 ? "Master's Degree" : "PhD"),
                    SalaryMin = 50000 + (i * 1000),
                    SalaryMax = 80000 + (i * 2000),
                    RequiredSkills = "Communication, Teamwork, Problem Solving",
                    PostedDate = DateTime.Now.AddDays(-i),
                    ExpiryDate = DateTime.Now.AddDays(30 - i),
                    IsActive = i % 5 != 0,
                    ViewCount = i * 15,
                    ApplicationCount = i * 3
                });
            }
            
            return jobPostings;
        }
        
        private List<Employer> GenerateMockEmployers()
        {
            var employers = new List<Employer>();
            
            for (int i = 1; i <= 10; i++)
            {
                employers.Add(new Employer
                {
                    Id = i,
                    CompanyName = $"Company {i}",
                    ContactPerson = $"Contact Person {i}",
                    Email = $"employer{i}@company{i}.com",
                    Industry = i % 5 == 0 ? "Technology" : (i % 5 == 1 ? "Healthcare" : (i % 5 == 2 ? "Finance" : (i % 5 == 3 ? "Education" : "Retail"))),
                    LogoUrl = $"/images/logos/logo{i % 8 + 1}.png"
                });
            }
            
            return employers;
        }
    }
} 