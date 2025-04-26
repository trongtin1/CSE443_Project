namespace CSE443_Project.Models
{
    public class Job
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Company { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
