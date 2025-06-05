namespace CSE443_Project.Models
{
    public class JobCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}