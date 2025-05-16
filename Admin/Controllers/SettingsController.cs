using Microsoft.AspNetCore.Mvc;
using Admin.Models.Settings;
using Microsoft.AspNetCore.Authorization;

namespace Admin.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ILogger<SettingsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new SettingsViewModel
            {
                // In a real application, these would be loaded from a database
                // But for demo purposes, we'll use mocked data
                SystemSettings = GetMockSystemSettings(),
                ProfileSettings = GetMockProfileSettings(),
                EmailSettings = GetMockEmailSettings()
            };
            
            return View(viewModel);
        }
        
        public IActionResult Profile()
        {
            var profileSettings = GetMockProfileSettings();
            return View(profileSettings);
        }
        
        [HttpPost]
        public IActionResult Profile(ProfileSettings model)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would save to database
                _logger.LogInformation("Profile settings updated by user {User}", User.Identity.Name);
                TempData["SuccessMessage"] = "Profile settings updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            
            return View(model);
        }
        
        public IActionResult System()
        {
            var systemSettings = GetMockSystemSettings();
            return View(systemSettings);
        }
        
        [HttpPost]
        public IActionResult System(SystemSettings model)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would save to database
                _logger.LogInformation("System settings updated by user {User}", User.Identity.Name);
                TempData["SuccessMessage"] = "System settings updated successfully!";
                return RedirectToAction(nameof(System));
            }
            
            return View(model);
        }
        
        public IActionResult Email()
        {
            var emailSettings = GetMockEmailSettings();
            return View(emailSettings);
        }
        
        [HttpPost]
        public IActionResult Email(EmailSettings model)
        {
            if (ModelState.IsValid)
            {
                // In a real application, you would save to database
                _logger.LogInformation("Email settings updated by user {User}", User.Identity.Name);
                TempData["SuccessMessage"] = "Email settings updated successfully!";
                return RedirectToAction(nameof(Email));
            }
            
            return View(model);
        }
        
        [HttpPost]
        public IActionResult SendTestEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToAction(nameof(Email));
            }
            
            // In a real application, you would actually send an email
            _logger.LogInformation("Test email would be sent to {EmailAddress}", emailAddress);
            TempData["SuccessMessage"] = $"Test email sent to {emailAddress}!";
            return RedirectToAction(nameof(Email));
        }
        
        // Mock data methods
        private SystemSettings GetMockSystemSettings()
        {
            return new SystemSettings
            {
                Id = 1,
                SiteName = "Job Portal Admin",
                SiteLogoUrl = "/images/logo.png",
                AdminEmail = "admin@jobportal.com",
                DefaultLanguage = "English",
                JobsPerPage = 20,
                EnableEmailNotifications = true,
                EnableJobApplicationSystem = true,
                MaintenanceMode = false,
                MaintenanceMessage = "The site is currently under maintenance. Please check back later.",
                JobPostingApprovalRequired = true,
                AllowPublicRegistration = true,
                DateFormat = "MM/dd/yyyy"
            };
        }
        
        private ProfileSettings GetMockProfileSettings()
        {
            return new ProfileSettings
            {
                Id = 1,
                FullName = "Admin User",
                Email = "admin@jobportal.com",
                ProfilePictureUrl = "https://via.placeholder.com/150",
                PhoneNumber = "123-456-7890",
                JobTitle = "System Administrator",
                Department = "IT Department",
                Bio = "Experienced system administrator with expertise in managing job portal systems.",
                Timezone = "UTC",
                ReceiveNotifications = true,
                TwoFactorAuth = false
            };
        }
        
        private EmailSettings GetMockEmailSettings()
        {
            return new EmailSettings
            {
                Id = 1,
                SmtpServer = "smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                FromEmail = "noreply@jobportal.com",
                FromName = "Job Portal",
                SmtpUsername = "noreply@jobportal.com",
                SmtpPassword = "********",
                EmailSignature = "<p>Regards,<br>Job Portal Team</p>",
                SendWelcomeEmail = true,
                SendJobPostingNotification = true,
                SendApplicationNotification = true,
                BccAdmin = false,
                AdminBccEmail = "admin@jobportal.com"
            };
        }
    }
} 