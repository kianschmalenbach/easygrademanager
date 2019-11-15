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

    public class TaskListDTO
    {
        public TaskListDTO(Task task)
        {
            if (task != null)
            {
                Id = task.Id;
                Number = task.Number;
                Name = task.Name;
                MaxScore = task.MaxScore;
            }
        }
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public double MaxScore { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is TaskListDTO && Id == ((TaskListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class TaskDetailDTO : TaskListDTO
    {
        public TaskDetailDTO(Task task, Evaluation evaluation) : base(task)
        {
            if (task != null)
            {
                Score = evaluation.Score;
            }
        }
        public double Score { get; set; }
    }
}
