namespace CSE443_Project.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Education { get; set; } = string.Empty;
        public string WorkExperience { get; set; } = string.Empty;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<SaveJob> SavedJobs { get; set; } = new List<SaveJob>();
        public ICollection<CV> CVs { get; set; } = new List<CV>();
        public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    }
}
