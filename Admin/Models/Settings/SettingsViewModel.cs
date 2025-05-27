namespace Admin.Models.Settings
{
    public class SettingsViewModel
    {
        public SystemSettings SystemSettings { get; set; }
        public ProfileSettings ProfileSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
        
        public SettingsViewModel()
        {
            SystemSettings = new SystemSettings
            {
                SiteName = "Job Portal",
                AdminEmail = "admin@example.com",
                DefaultLanguage = "en-US"
            };
            
            ProfileSettings = new ProfileSettings
            {
                FullName = "Administrator",
                Email = "admin@example.com"
            };
            
            EmailSettings = new EmailSettings
            {
                SmtpServer = "smtp.example.com",
                FromEmail = "noreply@example.com",
                FromName = "Job Portal"
            };
        }
    }
} 