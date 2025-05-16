using System.ComponentModel.DataAnnotations;

namespace Admin.Models.Settings
{
    public class SystemSettings
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Site name is required")]
        [StringLength(100, ErrorMessage = "Site name cannot exceed 100 characters")]
        public string SiteName { get; set; }
        
        [StringLength(255, ErrorMessage = "Site logo URL cannot exceed 255 characters")]
        public string SiteLogoUrl { get; set; }
        
        [Required(ErrorMessage = "Admin email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string AdminEmail { get; set; }
        
        [Required(ErrorMessage = "Default language is required")]
        public string DefaultLanguage { get; set; }
        
        [Range(5, 100, ErrorMessage = "Jobs per page must be between 5 and 100")]
        public int JobsPerPage { get; set; }
        
        public string DateFormat { get; set; }
        
        public bool EnableEmailNotifications { get; set; }
        
        public bool EnableJobApplicationSystem { get; set; }
        
        public bool JobPostingApprovalRequired { get; set; }
        
        public bool AllowPublicRegistration { get; set; }
        
        public bool MaintenanceMode { get; set; }
        
        [StringLength(500, ErrorMessage = "Maintenance message cannot exceed 500 characters")]
        public string MaintenanceMessage { get; set; }
    }
} 