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
        public int? GradingSchemeId { get; set; }
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
        public bool IsFinal()
        {
            if (Assignments == null)
                return false;
            foreach(Assignment assignment in Assignments)
            {
                if (!assignment.IsFinal)
                    return false;
            }
            return true;
        }
        public double GetAbsoluteScore(Student student)
        {
            if (student == null || Assignments == null)
                return 0;
            double score = 0;
            foreach (Assignment assignment in Assignments)
                score += assignment.GetScore(student);
            return score;
        }
        public double GetPercentage(Student student)
        {
            if (student == null || Assignments == null)
                return 0;
            double score = 0;
            double weightSum = 0;
            foreach (Assignment assignment in Assignments)
            {
                score += (assignment.GetScore(student) / assignment.GetMaxScore()) * assignment.Weight;
                weightSum += assignment.Weight;
            }
            return score / weightSum;
        }
        public bool HasPassed(Student student)
        {
            if (student == null)
                return false;
            if (GetAbsoluteScore(student) >= MinRequiredScore)
                return false;
            int passedAssignments = 0;
            if(Assignments != null)
            {
                foreach (Assignment assignment in Assignments)
                {
                    if (assignment.HasPassed(student))
                        passedAssignments++;
                    else if (assignment.Mandatory)
                        return false;
                }
            }
            return passedAssignments >= MinRequiredAssignments;
        }
        public string GetGrade(Student student)
        {
            if (student == null || Assignments == null || GradingScheme == null || 
                GradingScheme.Grades == null || GradingScheme.Grades.Count == 0)
                return null;
            double percentage = GetPercentage(student);
            if (percentage < 0 || percentage > 1)
                return null;
            return GradingScheme.GetGrade(percentage);
        }
        public ICollection<Student> GetStudents(Tutor tutor)
        {
            ICollection<Student> students = new HashSet<Student>();
            if (Assignments != null)
            {
                foreach (Assignment assignment in Assignments)
                {
                    foreach (Student student in assignment.GetStudents(tutor))
                        students.Add(student);
                }
            }
            return students;
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

    public class CourseDetailDTO: CourseListDTO
    {
        public CourseDetailDTO(Course course, Student student, Tutor tutor, Teacher teacher) : base(course)
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
                Final = course.IsFinal();
                if (course.IsFinal())
                {
                    if (student != null && course.GetStudents(null).Contains(student))
                        Result = new CourseResult(student, course, false);
                    else
                    {
                        Results = new HashSet<CourseResult>();
                        foreach (Student otherStudent in course.GetStudents(teacher != null ? null : tutor))
                            Results.Add(new CourseResult(otherStudent, course, true));
                    }
                }
            }
        }
        public bool Final { get; }
        public int MinRequiredAssignments { get; set; }
        public int MinRequiredScore { get; set; }
        public GradingSchemeDTO GradingScheme { get; set; }
        public UserListDTO Teacher { get; set; }
        public ICollection<AssignmentListDTO> Assignments { get; }
        public CourseResult Result { get; }
        public ICollection<CourseResult> Results { get; }
        public override bool Equals(object other)
        {
            return other != null && other is CourseDetailDTO && Id == ((CourseDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Course course)
        {
            int maxAssignments = 0;
            double maxScore = 0;
            if (course != null)
            {
                maxAssignments = course.Assignments.Count;
                foreach (Assignment assignment in course.Assignments)
                    foreach (Task task in assignment.Tasks)
                        maxScore += task.MaxScore;
            }
            return
                Name != null && Term != null &&
                MinRequiredAssignments >= 0 && MinRequiredAssignments <= maxAssignments &&
                MinRequiredScore >= 0 && MinRequiredScore <= maxScore;
        }
        public void Update(Course course)
        {
            course.Name = Name;
            course.Term = Term;
            course.Archived = Archived;
            course.MinRequiredAssignments = MinRequiredAssignments;
            course.MinRequiredScore = MinRequiredScore;
        }
        public Course Create(int teacherId)
        {
            Course course = new Course();
            Update(course);
            course.TeacherId = teacherId;
            return course;
        }
    }

    public class CourseResult
    {
        internal CourseResult(Student student, Course course, bool setStudent)
        {
            if (student != null && course != null)
            {
                if (setStudent)
                    Student = new UserListDTO(student.User);
                Id = student.Id;
                AbsoluteScore = course.GetAbsoluteScore(student);
                HasPassed = course.HasPassed(student);
                Percentage = course.GetPercentage(student);
                Grade = course.GetGrade(student);
            }
        }
        public int Id { get; }
        public UserListDTO Student { get; }
        public double AbsoluteScore { get; }
        public bool HasPassed { get; }
        public double Percentage { get; }
        public string Grade { get; }
    }
}
