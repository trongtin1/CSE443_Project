using System.ComponentModel.DataAnnotations;

namespace CSE443_Project.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public int FoundedYear { get; set; }
        public int CompanySize { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}