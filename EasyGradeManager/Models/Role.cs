using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGradeManager.Models
{
    public abstract class Role
    {
        public int Id { get; set; }
        [Required]
        public int AccountId { get; set; }
        public virtual User Account { get; set; }
    }

    [Table("Students")]
    public class Student : Role
    {
        public Student()
        {
            this.GroupMemberships = new HashSet<GroupMembership>();
        }
        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
    }

    [Table ("Teachers")]
    public class Teacher : Role
    {
        public Teacher()
        {
            this.Courses = new HashSet<Course>();
        }
        public virtual ICollection<Course> Courses { get; set; }
    }

    [Table("Tutors")]
    public class Tutor : Role
    {
        public Tutor()
        {
            this.Lessons = new HashSet<Lesson>();
        }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
