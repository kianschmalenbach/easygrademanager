using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public string GetGrade(double percentage)
        {
            if (Grades == null || Grades.Count == 0)
                return null;
            List<Grade> grades = Grades.OrderBy(grade => grade.MinPercentage).ToList();
            if (grades[0].MinPercentage != 0.0)
                return null;
            int index = 0;
            Grade cursor;
            do
            {
                cursor = grades[index];
                index++;
            } while (cursor.MinPercentage < percentage && index < grades.Count);
            return grades[index - 1].Name;
        }
    }

    public class GradingSchemeListDTO
    {
        public GradingSchemeListDTO(GradingScheme gradingScheme)
        {
            if (gradingScheme != null)
            {
                Id = gradingScheme.Id;
                Name = gradingScheme.Name;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GradingSchemeListDTO && Id == ((GradingSchemeListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GradingSchemeDetailDTO : GradingSchemeListDTO
    {
        public GradingSchemeDetailDTO(GradingScheme gradingScheme) : base(gradingScheme)
        {
            if (gradingScheme != null)
            {
                Grades = new HashSet<GradeDTO>();
                foreach (Grade grade in gradingScheme.Grades)
                    Grades.Add(new GradeDTO(grade));
            }
        }
        public ICollection<GradeDTO> Grades { get; set; }
        public int NewCourseId { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GradingSchemeDetailDTO && Id == ((GradingSchemeDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Teacher teacher)
        {
            HashSet<double> percentages = new HashSet<double>();
            HashSet<string> names = new HashSet<string>();
            bool encounteredZero = false;
            foreach (GradeDTO gradeDTO in Grades)
            {
                if (!gradeDTO.Validate())
                    return false;
                if (gradeDTO.MinPercentage == 0.0)
                    encounteredZero = true;
                percentages.Add(gradeDTO.MinPercentage);
                names.Add(gradeDTO.Name);
            }
            return
                Name != null && Name.Length <= 25 && GetCourse(teacher) != null &&
                percentages.Count == Grades.Count && names.Count == Grades.Count &&
                encounteredZero;
        }
        private void Update(GradingScheme scheme)
        {
            scheme.Name = Name;
            foreach (GradeDTO gradeDTO in Grades)
                scheme.Grades.Add(gradeDTO.Create());
        }
        public GradingScheme Create()
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
