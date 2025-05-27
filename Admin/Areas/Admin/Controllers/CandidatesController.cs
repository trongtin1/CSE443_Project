using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CandidatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CandidateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Add logic to save candidate
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, CandidateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Add logic to update candidate
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Add logic to delete candidate
            return RedirectToAction(nameof(Index));
        }
    }

    public class CandidateViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public int Experience { get; set; }
        public string Skills { get; set; }
        public string Notes { get; set; }
    }
} 