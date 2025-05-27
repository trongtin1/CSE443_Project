using System.ComponentModel.DataAnnotations;

namespace CSE443_Project.Models
{
    public class Job
    {
        [Key]
        public int job_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public decimal salary { get; set; }
        public string job_type { get; set; }
    }

}
