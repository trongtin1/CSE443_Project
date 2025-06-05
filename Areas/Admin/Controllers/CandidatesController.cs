using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CSE443_Project.Data;
using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandidatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            try
            {
                var query = _context.Candidates
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .Include(c => c.Job)
                    .Include(c => c.Application)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c =>
                        c.JobSeeker != null &&
                        c.JobSeeker.User != null && (
                            c.JobSeeker.User.Username.Contains(search) ||
                            c.JobSeeker.User.Email.Contains(search)
                        )
                    );
                }
                return View(query.ToList());
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error fetching candidates: {ex.Message}");

                // Check if error is related to missing table
                if (ex.Message.Contains("Invalid object name") || ex.InnerException?.Message.Contains("Invalid object name") == true)
                {
                    ViewBag.ErrorMessage = "The database schema is not properly set up. Please run migrations to create the required tables.";
                    return View(new List<Candidate>());
                }

                // Return empty list with error message for other errors
                ViewBag.ErrorMessage = "An error occurred while fetching candidates. Please try again later.";
                return View(new List<Candidate>());
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var candidate = _context.Candidates
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .Include(c => c.Job)
                    .Include(c => c.Application)
                    .FirstOrDefault(c => c.Id == id);

                if (candidate == null) return NotFound();
                return View(candidate);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the candidate details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            try
            {
                // Load job seekers for dropdown
                var jobSeekers = _context.JobSeekers
                    .Include(js => js.User)
                    .ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username");

                // Load jobs for dropdown
                var jobs = _context.Jobs
                    .Where(j => j.IsActive && j.Deadline >= DateTime.Now)
                    .ToList();
                ViewBag.Jobs = new SelectList(jobs, "Id", "JobTitle");

                // Load applications for dropdown (optional, as it will be filtered based on JobSeeker and Job selection)
                var applications = _context.Applications
                    .Include(a => a.JobSeeker)
                    .ThenInclude(js => js.User)
                    .Include(a => a.Job)
                    .Where(a => a.Status == "Pending" || a.Status == "Reviewed")
                    .ToList();

                // Create a formatted list of applications with job seeker and job info
                var formattedApplications = applications.Select(a => new
                {
                    Id = a.Id,
                    DisplayText = $"[{a.Id}] {a.JobSeeker?.User?.Username ?? "Unknown"} - {a.Job?.JobTitle ?? "Unknown Job"}"
                }).ToList();

                ViewBag.Applications = new SelectList(formattedApplications, "Id", "DisplayText");

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while preparing the creation form: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Candidate model)
        {
            try
            {
                // Debug: Log các giá trị nhận được
                Console.WriteLine($"ApplicationId: {model.ApplicationId}, JobId: {model.JobId}, JobSeekerId: {model.JobSeekerId}");

                // Nếu Application, lấy JobId và JobSeekerId từ Application
                if (model.ApplicationId.HasValue && model.ApplicationId.Value > 0)
                {
                    var application = _context.Applications.Find(model.ApplicationId.Value);
                    if (application != null)
                    {
                        model.JobId = application.JobId;
                        model.JobSeekerId = application.JobSeekerId;
                    }
                    else
                    {
                        ModelState.AddModelError("ApplicationId", "Selected application does not exist.");
                    }
                }
                // Thủ công phải có cả JobId và JobSeekerId
                else if (!model.JobId.HasValue || !model.JobSeekerId.HasValue || model.JobId.Value == 0 || model.JobSeekerId.Value == 0)
                {
                    ModelState.AddModelError("", "Please select either an application or both job and job seeker.");
                }

                // Debug: Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    ViewBag.DebugModelState = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                if (ModelState.IsValid)
                {
                    model.ShortlistedDate = DateTime.Now;
                    _context.Candidates.Add(model);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }

                // Reload dropdowns nếu có lỗi
                var jobSeekers = _context.JobSeekers.Include(js => js.User).ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username", model.JobSeekerId);

                var jobs = _context.Jobs.Where(j => j.IsActive && j.Deadline >= DateTime.Now).ToList();
                ViewBag.Jobs = new SelectList(jobs, "Id", "JobTitle", model.JobId);

                var applications = _context.Applications
                    .Include(a => a.JobSeeker).ThenInclude(js => js.User)
                    .Include(a => a.Job)
                    .Where(a => a.Status == "Pending" || a.Status == "Reviewed")
                    .ToList();
                var formattedApplications = applications.Select(a => new
                {
                    Id = a.Id,
                    DisplayText = $"[{a.Id}] {a.JobSeeker?.User?.Username ?? "Unknown"} - {a.Job?.JobTitle ?? "Unknown Job"}"
                }).ToList();
                ViewBag.Applications = new SelectList(formattedApplications, "Id", "DisplayText", model.ApplicationId);

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while creating the candidate: " + ex.Message;
                // Reload dropdowns
                var jobSeekers = _context.JobSeekers.Include(js => js.User).ToList();
                ViewBag.JobSeekers = new SelectList(jobSeekers, "Id", "User.Username", model.JobSeekerId);

                var jobs = _context.Jobs.Where(j => j.IsActive && j.Deadline >= DateTime.Now).ToList();
                ViewBag.Jobs = new SelectList(jobs, "Id", "JobTitle", model.JobId);

                var applications = _context.Applications
                    .Include(a => a.JobSeeker).ThenInclude(js => js.User)
                    .Include(a => a.Job)
                    .Where(a => a.Status == "Pending" || a.Status == "Reviewed")
                    .ToList();
                var formattedApplications = applications.Select(a => new
                {
                    Id = a.Id,
                    DisplayText = $"[{a.Id}] {a.JobSeeker?.User?.Username ?? "Unknown"} - {a.Job?.JobTitle ?? "Unknown Job"}"
                }).ToList();
                ViewBag.Applications = new SelectList(formattedApplications, "Id", "DisplayText", model.ApplicationId);

                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var candidate = _context.Candidates
                    .Include(c => c.JobSeeker)
                    .ThenInclude(js => js.User)
                    .Include(c => c.Job)
                    .Include(c => c.Application)
                    .FirstOrDefault(c => c.Id == id);

                if (candidate == null) return NotFound();

                // Load job seekers for dropdown (read-only in edit mode)
                ViewBag.JobSeekerName = candidate.JobSeeker?.User?.Username ?? "Unknown";

                // Load job title (read-only in edit mode)
                ViewBag.JobTitle = candidate.Job?.JobTitle ?? "Unknown Job";

                // Status options
                ViewBag.StatusOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Shortlisted", Text = "Shortlisted" },
                    new SelectListItem { Value = "Interviewed", Text = "Interviewed" },
                    new SelectListItem { Value = "Hired", Text = "Hired" },
                    new SelectListItem { Value = "Rejected", Text = "Rejected" }
                };

                return View(candidate);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the candidate for editing: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Candidate model)
        {
            try
            {
                if (id != model.Id) return BadRequest();

                // Get existing entity to preserve values we don't want to change
                var existingCandidate = _context.Candidates.Find(id);
                if (existingCandidate == null) return NotFound();

                // Only update specific fields
                existingCandidate.Status = model.Status;
                existingCandidate.InterviewNotes = model.InterviewNotes;
                existingCandidate.InterviewDate = model.InterviewDate;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the candidate: " + ex.Message;

                // Reload dropdown data
                ViewBag.StatusOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Shortlisted", Text = "Shortlisted" },
                    new SelectListItem { Value = "Interviewed", Text = "Interviewed" },
                    new SelectListItem { Value = "Hired", Text = "Hired" },
                    new SelectListItem { Value = "Rejected", Text = "Rejected" }
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var candidate = _context.Candidates.Find(id);
                if (candidate != null)
                {
                    _context.Candidates.Remove(candidate);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the candidate: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}