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

    [Table("Teachers")]
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
        public RoleDTO(Role role)
        {
            if (role != null)
            {
                Courses = new HashSet<CourseListDTO>();
                Id = role.Id;
            }
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
        public TeacherDTO(Teacher teacher) : base(teacher)
        {
            if (teacher != null)
            {
                foreach (Course course in teacher.Courses)
                    Courses.Add(new CourseListDTO(course));
            }
        }
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
        public TutorDTO(Tutor tutor) : base(tutor)
        {
            if (tutor != null)
            {
                Lessons = new HashSet<LessonListDTO>();
                foreach (Lesson lesson in tutor.Lessons)
                {
                    Lessons.Add(new LessonListDTO(lesson, tutor));
                    Courses.Add(new CourseListDTO(lesson.Assignment.Course));
                }
            }
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
        public StudentDTO(Student student) : base(student)
        {
            if (student != null)
            {
                Groups = new HashSet<GroupListDTO>();
                foreach (GroupMembership membership in student.GroupMemberships)
                {
                    Groups.Add(new GroupListDTO(membership.Group));
                    Courses.Add(new CourseListDTO(membership.Group.Lesson.Assignment.Course));
                }
            }
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
