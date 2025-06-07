namespace CSE443_Project.Models
{
    public class Job
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public int CategoryId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string Benefits { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Contract, etc.
        public string ExperienceLevel { get; set; } = string.Empty; // Entry, Mid, Senior
        public string Location { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public bool IsRemote { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int Vacancies { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime Deadline { get; set; }

        // Navigation properties
        public Employer? Employer { get; set; }
        public JobCategory? Category { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<SaveJob> SavedBy { get; set; } = new List<SaveJob>();
        public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}
