using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Course
    {
        public Course()
        {
            this.Archived = false;
            this.Assignments = new HashSet<Assignment>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Term { get; set; }
        public bool Archived { get; set; }
        public int MinRequiredAssignments { get; set; }
        public int MinRequiredScore { get; set; }
        public IDictionary<double, string> GradingScheme { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
