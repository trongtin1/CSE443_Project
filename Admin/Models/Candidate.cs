using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public string Skills { get; set; }
        public string ResumeUrl { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
        public int JobApplicationsCount { get; set; }
    }
} 