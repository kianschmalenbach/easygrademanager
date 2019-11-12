using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Task
    {
        public Task()
        {
            Evaluations = new HashSet<Evaluation>();
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double MaxScore { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Task && Id == ((Task)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class TaskDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public double MaxScore { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is TaskDTO && Id == ((TaskDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
