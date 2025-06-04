namespace CSE443_Project.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int JobSeekerId { get; set; }
        public string Status { get; set; } = "Shortlisted"; // Shortlisted, Interviewed, Hired, Rejected
        public string? InterviewNotes { get; set; }
        public DateTime? InterviewDate { get; set; }
        public DateTime ShortlistedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Application Application { get; set; } = null!;
        public Job Job { get; set; } = null!;
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}