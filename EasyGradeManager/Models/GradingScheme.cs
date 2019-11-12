using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class GradingScheme
    {
        public GradingScheme()
        {
            MinScores = new List<double>();
            Grades = new List<string>();
            Courses = new List<Course>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<double> MinScores { get; }
        public List<string> Grades { get; }
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
        public GradingSchemeDTO()
        {
            MinScores = new List<double>();
            Grades = new List<string>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<double> MinScores { get; }
        public List<string> Grades { get; }
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