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
        public int NewAssignmentId { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is TaskDetailDTO && Id == ((TaskDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Task task, Assignment assignment)
        {
            if (assignment == null || assignment.Tasks == null)
                return false;
            HashSet<int> numbers = new HashSet<int>();
            HashSet<string> names = new HashSet<string>();
            foreach (Task otherTask in assignment.Tasks)
            {
                numbers.Add(otherTask.Number);
                names.Add(otherTask.Name);
            }
            if(task != null)
            {
                numbers.Remove(task.Number);
                names.Remove(task.Name);
            }
            return 
                (task == null || NewAssignmentId == 0) && !numbers.Contains(Number) && !names.Contains(Name) && 
                MaxScore > 0;
        }
        public void Update(Task task)
        {
            task.MaxScore = MaxScore;
            task.Name = Name;
            task.Number = Number;
        }
        public Task Create()
        {
            Task task = new Task();
            Update(task);
            task.AssignmentId = NewAssignmentId;
            return task;
        }
    }
}
