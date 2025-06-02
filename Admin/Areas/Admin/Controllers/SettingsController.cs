using Microsoft.AspNetCore.Mvc;
using Admin.Models.Settings;

namespace Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new SettingsViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Success"] = "Settings saved successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
