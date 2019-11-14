using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Evaluation
    {
        public Evaluation()
        {
            Score = 0.0;
        }
        public int Id { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        public double Score { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is Evaluation && Id == ((Evaluation)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class EvaluationDTO
    {
        public EvaluationDTO(Evaluation evaluation)
        {
            Id = evaluation.Id;
            Score = evaluation.Score;
            if (evaluation.Task != null)
                Task = new TaskDTO(evaluation.Task);
        }
        public int Id { get; set; }
        public double Score { get; set; }
        public TaskDTO Task { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is EvaluationDTO && Id == ((EvaluationDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
