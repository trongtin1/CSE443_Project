using System.ComponentModel.DataAnnotations;

namespace Admin.Models.Settings
{
    public class SystemSettings
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Site name is required")]
        [StringLength(100, ErrorMessage = "Site name cannot exceed 100 characters")]
        public required string SiteName { get; set; }
        
        [StringLength(255, ErrorMessage = "Site logo URL cannot exceed 255 characters")]
        public string SiteLogoUrl { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Admin email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string AdminEmail { get; set; }
        
        [Required(ErrorMessage = "Default language is required")]
        public required string DefaultLanguage { get; set; }
        
        [Range(5, 100, ErrorMessage = "Jobs per page must be between 5 and 100")]
        public int JobsPerPage { get; set; } = 10;
        
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        
        public bool EnableEmailNotifications { get; set; } = true;
        
        public bool EnableJobApplicationSystem { get; set; } = true;
        
        public bool JobPostingApprovalRequired { get; set; } = true;
        
        public bool AllowPublicRegistration { get; set; } = true;
        
        public bool MaintenanceMode { get; set; } = false;
        
        [StringLength(500, ErrorMessage = "Maintenance message cannot exceed 500 characters")]
        public string MaintenanceMessage { get; set; } = "System is under maintenance. Please try again later.";
    }
} 