using System.ComponentModel.DataAnnotations;

namespace CSE443_Project.Models
{
    public class CV
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }

        [Required(ErrorMessage = "CV Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}
