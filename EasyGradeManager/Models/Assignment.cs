using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Assignment
    {
        public Assignment()
        {
            IsFinal = false;
            Lessons = new HashSet<Lesson>();
            Tasks = new HashSet<Task>();
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
        public virtual ICollection<Lesson> Lessons { get; }
        public virtual ICollection<Task> Tasks { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Assignment && Id == ((Assignment)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class AssignmentListDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Deadline { get; set; }
        public double MinRequiredScore { get; set; }
        public bool Mandatory { get; set; }
        public double Weight { get; set; }
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public bool IsFinal { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentListDTO && Id == ((AssignmentListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public abstract class AssignmentDetailDTO : AssignmentListDTO
    {
        protected AssignmentDetailDTO()
        {
            Tasks = new HashSet<TaskDTO>();
        }
        public ICollection<TaskDTO> Tasks { get; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailDTO && Id == ((AssignmentDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class AssignmentDetailTeacherDTO : AssignmentDetailDTO
    {
        public AssignmentDetailTeacherDTO()
        {
            Lessons = new HashSet<LessonListDTO>();
        }
        public ICollection<LessonListDTO> Lessons { get; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailTeacherDTO && Id == ((AssignmentDetailTeacherDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class AssignmentDetailStudentDTO : AssignmentDetailDTO
    {
        public AssignmentDetailStudentDTO()
        {
            Evaluations = new HashSet<EvaluationDTO>();
        }
        public LessonListDTO Lesson { get; set; }
        public GroupDetailDTO Group { get; set; }
        public ICollection<EvaluationDTO> Evaluations { get; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailStudentDTO && Id == ((AssignmentDetailStudentDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
