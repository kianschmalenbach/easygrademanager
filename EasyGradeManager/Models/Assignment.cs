using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Assignment
    {
        public Assignment()
        {
            this.IsFinal = false;
            this.Groups = new HashSet<Group>();
            this.Lessons = new HashSet<Lesson>();
            this.Tasks = new HashSet<Task>();
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Name { get; set; }
        public string Deadline { get; set; }
        public double MinRequiredScore { get; set; }
        public bool Mandatory { get; set; }
        public double Weight { get; set; }
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public bool IsFinal { get; set; }
        [Required]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
