using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; } // Full-time, Part-time, Contract, etc.
        public string ExperienceLevel { get; set; }
        public string EducationLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string RequiredSkills { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int ApplicationCount { get; set; }
        
        // Navigation property
        public Employer Employer { get; set; }
    }
} 