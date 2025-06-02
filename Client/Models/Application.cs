namespace CSE443_Project.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int JobSeekerId { get; set; }
        public int? CVId { get; set; }
        public DateTime ApplicationDate { get; set; } = DateTime.Now;
        public string CoverLetter { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Shortlisted, Interviewed, Accepted, Rejected
        public string? Notes { get; set; }
        public DateTime? ReviewDate { get; set; }

        // Navigation properties
        public Job Job { get; set; } = null!;
        public JobSeeker JobSeeker { get; set; } = null!;
        public CV? CV { get; set; }
    }
}
