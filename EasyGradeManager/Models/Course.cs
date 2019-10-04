using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyGradeManager.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public String CourseName { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}