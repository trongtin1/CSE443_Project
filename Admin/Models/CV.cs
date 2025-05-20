using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Admin.Models
{
    public class CV
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người dùng")]
        [Display(Name = "Người dùng")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Nội dung CV không được để trống")]
        [Display(Name = "Nội dung CV")]
        public string Content { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}