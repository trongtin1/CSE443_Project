 using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Position { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [Range(0, 50)]
        public int Experience { get; set; }

        [StringLength(500)]
        public string Skills { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
