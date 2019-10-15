using System;
using System.Collections.Generic;

namespace EasyGradeManager.Models
{
    public class Course
    {
        public int Id { get; set; }
        //[Required]
        public String Name { get; set; }
        public ICollection<Student> Students { get; set; }
        public int MinAllowedGroupSize { get; set; }
        public int MaxAllowedGroupSize { get; set; }
        public ICollection<EvaluationCriterion> EvaluationCriteria { get; set; }

    }
}
