using Microsoft.AspNetCore.Mvc;
using Admin.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Admin.Controllers
{
    [Authorize]
    public class EmployersController : Controller
    {
        private readonly ILogger<EmployersController> _logger;

        public EmployersController(ILogger<EmployersController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // For demo purposes, creating mock employers
            // In a real application, you would fetch this data from a database
            var employers = GetMockEmployers();
            return View(employers);
        }

        public IActionResult Details(int id)
        {
            // In a real application, you would fetch the employer from a database
            var employer = GetMockEmployers().FirstOrDefault(e => e.Id == id);
            
            if (employer == null)
            {
                return NotFound();
            }
            
            return View(employer);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employer = GetMockEmployers().FirstOrDefault(e => e.Id == id);
            
            if (employer == null)
            {
                return NotFound();
            }
            
            return View(employer);
        }

        [HttpPost]
        public IActionResult Edit(Employer employer)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would update the employer in a database
                TempData["SuccessMessage"] = "Employer updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(employer);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var employer = GetMockEmployers().FirstOrDefault(e => e.Id == id);
            
            if (employer == null)
            {
                return NotFound();
            }
            
            return View(employer);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // In a real application, you would delete the employer from a database
            TempData["SuccessMessage"] = "Employer deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ToggleVerification(int id)
        {
            // In a real application, you would toggle the verification status in a database
            TempData["SuccessMessage"] = "Employer verification status updated!";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to generate mock employers for demo
        private List<Employer> GetMockEmployers()
        {
            var employers = new List<Employer>();
            
            for (int i = 1; i <= 20; i++)
            {
                employers.Add(new Employer
                {
                    Id = i,
                    CompanyName = $"Company {i}",
                    ContactPerson = $"Contact Person {i}",
                    Email = $"employer{i}@company{i}.com",
                    Phone = $"123-789-{i.ToString().PadLeft(4, '0')}",
                    Address = $"Company Address {i}, Business District",
                    Industry = i % 5 == 0 ? "Technology" : (i % 5 == 1 ? "Healthcare" : (i % 5 == 2 ? "Finance" : (i % 5 == 3 ? "Education" : "Retail"))),
                    CompanySize = i % 4 == 0 ? "1-10" : (i % 4 == 1 ? "11-50" : (i % 4 == 2 ? "51-200" : "201+")),
                    Website = $"https://company{i}.com",
                    Description = $"Description for Company {i}. This is a company in the {(i % 5 == 0 ? "Technology" : (i % 5 == 1 ? "Healthcare" : (i % 5 == 2 ? "Finance" : (i % 5 == 3 ? "Education" : "Retail"))))} industry.",
                    LogoUrl = $"/images/logos/logo{i % 8 + 1}.png",
                    RegisteredDate = DateTime.Now.AddMonths(-(i % 12)),
                    IsVerified = i % 3 != 0,
                    IsActive = i % 7 != 0,
                    JobPostingsCount = i * 2
                });
            }
            
            return employers;
        }
    }
} 