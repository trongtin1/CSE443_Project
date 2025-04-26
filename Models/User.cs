namespace CSE443_Project.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }  // "Admin", "Candidate", "Recruiter"
    }
}
