namespace Admin.Models.Settings
{
    public class SettingsViewModel
    {
        public SystemSettings SystemSettings { get; set; }
        public ProfileSettings ProfileSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
        
        public SettingsViewModel()
        {
            SystemSettings = new SystemSettings();
            ProfileSettings = new ProfileSettings();
            EmailSettings = new EmailSettings();
        }
    }
} 