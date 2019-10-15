using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyGradeManager.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public String CourseName { get; set; }
        public ICollection<Student> Students { get; set; }
        
        private string name;

        private int minAllowedGroupSize;

        private int maxAllowedGroupSize;

        private Collection<EvaluationCriterion> evaluationCriteria;
    }
}