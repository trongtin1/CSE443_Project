namespace CSE443_Project.Models
{
    public class SaveJob
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public int JobId { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;
        public string? Notes { get; set; }

        // Navigation properties
        public JobSeeker JobSeeker { get; set; } = null!;
        public Job Job { get; set; } = null!;
    }
}
