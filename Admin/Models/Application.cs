using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        public int CVId { get; set; }
        [ForeignKey("CVId")]
        public virtual CV CV { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [StringLength(1000)]
        public string CoverLetter { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNotes { get; set; }
    }
}
