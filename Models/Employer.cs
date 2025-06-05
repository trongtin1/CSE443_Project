using System.ComponentModel.DataAnnotations;

namespace CSE443_Project.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
        public string CompanySize { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public DateTime RegisteredDate { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public int JobPostingsCount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}