using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGradeManager.Models
{
    public abstract class Role
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Name { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is Role && Id == ((Role)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    [Table ("Teachers")]
    public class Teacher : Role
    {
        public Teacher()
        {
            Courses = new HashSet<Course>();
            Name = "Teacher";
        }
        public virtual ICollection<Course> Courses { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Teacher && Id == ((Teacher)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    [Table("Tutors")]
    public class Tutor : Role
    {
        public Tutor()
        {
            Lessons = new HashSet<Lesson>();
            Name = "Tutor";
        }
        public virtual ICollection<Lesson> Lessons { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Tutor && Id == ((Tutor)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    [Table("Students")]
    public class Student : Role
    {
        public Student()
        {
            GroupMemberships = new HashSet<GroupMembership>();
            Name = "Student";
        }
        public virtual ICollection<GroupMembership> GroupMemberships { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Student && Id == ((Student)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class RoleDTO
    {
        public RoleDTO()
        {
            Courses = new HashSet<CourseListDTO>();
        }
        public int Id { get; set; }
        public ICollection<CourseListDTO> Courses { get; }
        public override bool Equals(object other)
        {
            return other != null && other is RoleDTO && Id == ((RoleDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class TeacherDTO : RoleDTO
    {
        public override bool Equals(object other)
        {
            return other != null && other is TeacherDTO && Id == ((TeacherDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class TutorDTO : RoleDTO
    {
        public TutorDTO()
        {
            Lessons = new HashSet<LessonListDTO>();
        }
        public ICollection<LessonListDTO> Lessons { get; }
        public override bool Equals(object other)
        {
            return other != null && other is TutorDTO && Id == ((TutorDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class StudentDTO : RoleDTO
    {
        public StudentDTO()
        {
            Groups = new HashSet<GroupListDTO>();
        }
        public ICollection<GroupListDTO> Groups { get; }
        public override bool Equals(object other)
        {
            return other != null && other is StudentDTO && Id == ((StudentDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
