using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Company { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        [StringLength(100)]
        public string Industry { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMax { get; set; }

        [Required]
        public string EmploymentType { get; set; } // Full-time, Part-time, Contract, etc.

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
    }
}
