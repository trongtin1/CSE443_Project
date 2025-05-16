namespace CSE443_Project.Models
{
    public class User
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public string role { get; set; } //
        public DateTime created_at { get; set; }
    }

}
