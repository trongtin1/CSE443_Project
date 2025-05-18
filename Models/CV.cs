namespace CSE443_Project.Models
{

    public class CV
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Content { get; set; } // hoặc FilePath nếu dùng file
        public DateTime CreatedAt { get; set; }

        public required User User { get; set; }
    }
}