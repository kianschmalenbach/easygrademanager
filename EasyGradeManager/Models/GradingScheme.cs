using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGradeManager.Models
{
    public class GradingScheme
    {
        public GradingScheme()
        {
            Grades = new HashSet<Grade>();
            Courses = new List<Course>();
        }
        public int Id { get; set; }
        [Required, Index(IsUnique = true), StringLength(25)]
        public string Name { get; set; }
        public virtual ICollection<Grade> Grades { get; }
        public virtual ICollection<Course> Courses { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GradingScheme && Id == ((GradingScheme)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GradingSchemeDTO
    {
        public GradingSchemeDTO(GradingScheme gradingScheme)
        {
            if (gradingScheme != null)
            {
                Grades = new HashSet<GradeDTO>();
                Id = gradingScheme.Id;
                Name = gradingScheme.Name;
                foreach (Grade grade in gradingScheme.Grades)
                    Grades.Add(new GradeDTO(grade));
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<GradeDTO> Grades { get; set; }
        public int NewCourseId { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GradingSchemeDTO && Id == ((GradingSchemeDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Teacher teacher)
        {
            HashSet<double> percentages = new HashSet<double>();
            HashSet<string> names = new HashSet<string>();
            foreach (GradeDTO gradeDTO in Grades)
            {
                if (!gradeDTO.Validate())
                    return false;
                percentages.Add(gradeDTO.MinPercentage);
                names.Add(gradeDTO.Name);
            }
            return 
                Name != null && Name.Length <= 25 && GetCourse(teacher) != null && 
                percentages.Count == Grades.Count && names.Count == Grades.Count;
        }
        private void Update(GradingScheme scheme)
        {
            scheme.Name = Name;
            foreach (GradeDTO gradeDTO in Grades)
                scheme.Grades.Add(gradeDTO.Create());
        }
        public GradingScheme Create(Teacher teacher)
        {
            GradingScheme scheme = new GradingScheme();
            Update(scheme);
            return scheme;
        }
        private Course GetCourse(Teacher teacher)
        {
            foreach (Course course in teacher.Courses)
            {
                if (course.Id == NewCourseId)
                    return course;
            }
            return null;
        }
    }
}
