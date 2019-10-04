using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyGradeManager.Models
{
    [Table("Étudiant")]
    public class Student
    {
        public int Id { get; set; } //primary key by convention
        [Required] //next property is NOT NULL
        public string Name { get; set; }
        public string FirstName { get; set; }
        [Required]
        public int Age { get; set; }
    }
}