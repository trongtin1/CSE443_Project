using System.ComponentModel.DataAnnotations;

namespace Admin.Models.Settings
{
    public class EmailSettings
    {
        public int Id { get; set; }
        
        [Display(Name = "SMTP Server")]
        [Required(ErrorMessage = "SMTP server is required")]
        public string SmtpServer { get; set; }
        
        [Display(Name = "SMTP Port")]
        [Required(ErrorMessage = "SMTP port is required")]
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
        public int SmtpPort { get; set; } = 587;
        
        [Display(Name = "Enable SSL")]
        public bool EnableSsl { get; set; } = true;
        
        [Display(Name = "From Email Address")]
        [Required(ErrorMessage = "From email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string FromEmail { get; set; }
        
        [Display(Name = "From Name")]
        [Required(ErrorMessage = "From name is required")]
        public string FromName { get; set; }
        
        [Display(Name = "SMTP Username")]
        public string SmtpUsername { get; set; }
        
        [Display(Name = "SMTP Password")]
        [DataType(DataType.Password)]
        public string SmtpPassword { get; set; }
        
        [Display(Name = "Email Signature")]
        public string EmailSignature { get; set; }
        
        [Display(Name = "Send Welcome Email to New Users")]
        public bool SendWelcomeEmail { get; set; } = true;
        
        [Display(Name = "Send Notification on New Job Posting")]
        public bool SendJobPostingNotification { get; set; } = true;
        
        [Display(Name = "Send Notification on New Application")]
        public bool SendApplicationNotification { get; set; } = true;
        
        [Display(Name = "BCC Admin on All Emails")]
        public bool BccAdmin { get; set; } = false;
        
        [Display(Name = "Admin Email Address for BCC")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string AdminBccEmail { get; set; }
    }
} 