using System;
using System.ComponentModel.DataAnnotations;

namespace CSE443_Project.Models
{
    public class Application
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int job_id { get; set; }
        public int cv_id { get; set; }

        public string status { get; set; }
        public DateTime applied_at { get; set; }
    }
}
