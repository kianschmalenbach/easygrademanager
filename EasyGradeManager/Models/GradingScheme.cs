using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required]
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
            Grades = new HashSet<GradeDTO>();
            Id = gradingScheme.Id;
            Name = gradingScheme.Name;
            foreach (Grade grade in gradingScheme.Grades)
                Grades.Add(new GradeDTO(grade));
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<GradeDTO> Grades { get; }
        public override bool Equals(object other)
        {
            return other != null && other is GradingSchemeDTO && Id == ((GradingSchemeDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}