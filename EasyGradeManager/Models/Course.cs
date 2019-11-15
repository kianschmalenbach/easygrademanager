using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Course
    {
        public Course()
        {
            Archived = false;
            Assignments = new HashSet<Assignment>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Term { get; set; }
        public bool Archived { get; set; }
        public int MinRequiredAssignments { get; set; }
        public int MinRequiredScore { get; set; }
        public int GradingSchemeId { get; set; }
        public virtual GradingScheme GradingScheme { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Assignment> Assignments { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Course && Id == ((Course)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class CourseListDTO
    {
        public CourseListDTO(Course course)
        {
            if (course != null)
            {
                Id = course.Id;
                Name = course.Name;
                Term = course.Term;
                Archived = course.Archived;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Term { get; set; }
        public bool Archived { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is CourseListDTO && Id == ((CourseListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class CourseDetailDTO : CourseListDTO
    {
        public CourseDetailDTO(Course course) : base(course)
        {
            if (course != null)
            {
                Assignments = new HashSet<AssignmentListDTO>();
                MinRequiredAssignments = course.MinRequiredAssignments;
                MinRequiredScore = course.MinRequiredScore;
                if (course.GradingScheme != null)
                    GradingScheme = new GradingSchemeDTO(course.GradingScheme);
                if (course.Teacher != null)
                    Teacher = new UserListDTO(course.Teacher.User);
                foreach (Assignment assignment in course.Assignments)
                    Assignments.Add(new AssignmentListDTO(assignment));
            }
        }
        public int MinRequiredAssignments { get; set; }
        public int MinRequiredScore { get; set; }
        public GradingSchemeDTO GradingScheme { get; set; }
        public UserListDTO Teacher { get; set; }
        public ICollection<AssignmentListDTO> Assignments { get; }
        public override bool Equals(object other)
        {
            return other != null && other is CourseDetailDTO && Id == ((CourseDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
