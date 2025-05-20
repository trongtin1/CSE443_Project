using Microsoft.AspNetCore.Mvc;
using Admin.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Admin.Controllers
{
    [Authorize]
    public class CandidatesController : Controller
    {
        private readonly ILogger<CandidatesController> _logger;

        public CandidatesController(ILogger<CandidatesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // For demo purposes, creating mock candidates
            // In a real application, you would fetch this data from a database
            var candidates = GetMockCandidates();
            return View(candidates);
        }

        public IActionResult Details(int id)
        {
            // In a real application, you would fetch the candidate from a database
            var candidate = GetMockCandidates().FirstOrDefault(c => c.Id == id);
            
            if (candidate == null)
            {
                return NotFound();
            }
            
            return View(candidate);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var candidate = GetMockCandidates().FirstOrDefault(c => c.Id == id);
            
            if (candidate == null)
            {
                return NotFound();
            }
            
            return View(candidate);
        }

        [HttpPost]
        public IActionResult Edit(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would update the candidate in a database
                TempData["SuccessMessage"] = "Candidate updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var candidate = GetMockCandidates().FirstOrDefault(c => c.Id == id);
            
            if (candidate == null)
            {
                return NotFound();
            }
            
            return View(candidate);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // In a real application, you would delete the candidate from a database
            TempData["SuccessMessage"] = "Candidate deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Helper method to generate mock candidates for demo
        private List<Candidate> GetMockCandidates()
        {
            var candidates = new List<Candidate>();
            
            for (int i = 1; i <= 25; i++)
            {
                candidates.Add(new Candidate
                {
                    Id = i,
                    FullName = $"Candidate {i}",
                    Email = $"candidate{i}@example.com",
                    Phone = $"123-456-{i.ToString().PadLeft(4, '0')}",
                    Address = $"Address {i}, City",
                    DateOfBirth = DateTime.Now.AddYears(-20 - (i % 15)),
                    Education = i % 3 == 0 ? "Bachelor's Degree" : (i % 3 == 1 ? "Master's Degree" : "PhD"),
                    Experience = $"{i % 10} years",
                    Skills = "C#, ASP.NET, SQL, JavaScript",
                    ResumeUrl = "/uploads/resumes/resume" + i + ".pdf",
                    ProfilePictureUrl = "/images/avatars/avatar" + (i % 5 + 1) + ".jpg",
                    RegisteredDate = DateTime.Now.AddDays(-i * 5),
                    IsActive = i % 5 != 0,
                    JobApplicationsCount = i % 10
                });
            }
            
            return candidates;
        }
    }
} 