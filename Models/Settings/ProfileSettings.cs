using System.ComponentModel.DataAnnotations;

namespace Admin.Models.Settings
{
    public class ProfileSettings
    {
        public int Id { get; set; }
        
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full name is required")]
        public required string FullName { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }
        
        [Display(Name = "Profile Picture URL")]
        public string ProfilePictureUrl { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;
        
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
        
        [Display(Name = "Bio")]
        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string Bio { get; set; } = string.Empty;
        
        [Display(Name = "Timezone")]
        public string Timezone { get; set; } = "UTC";
        
        [Display(Name = "Receive Notifications")]
        public bool ReceiveNotifications { get; set; } = true;
        
        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorAuth { get; set; } = false;
        
        // Password change fields - not stored but used for form submission
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
} 