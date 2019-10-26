using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Evaluation
    {
        public Evaluation()
        {
            this.Score = 0.0;
            this.IsFinal = false;
        }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        [Required]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
        public double Score { get; set; }
        public bool IsFinal { get; set; }
    }
}
