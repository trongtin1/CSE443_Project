namespace CSE443_Project.Models
{
    public class CV
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}
