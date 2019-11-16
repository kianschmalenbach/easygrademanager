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
        public AssignmentListDTO(Assignment assignment)
        {
            if (assignment != null)
            {
                Id = assignment.Id;
                Deadline = assignment.Deadline;
                IsFinal = assignment.IsFinal;
                Mandatory = assignment.Mandatory;
                MaxGroupSize = assignment.MaxGroupSize;
                MinGroupSize = assignment.MinGroupSize;
                MinRequiredScore = assignment.MinRequiredScore;
                Name = assignment.Name;
                Number = assignment.Number;
                Weight = assignment.Weight;
            }
        }
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
        protected AssignmentDetailDTO(Assignment assignment) : base(assignment)
        {
            if (assignment != null)
            {
                if (assignment.Course != null)
                    Course = new CourseListDTO(assignment.Course);
            }
        }
        public CourseListDTO Course { get; set; }
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
        public AssignmentDetailTeacherDTO(Assignment assignment) : base(assignment)
        {
            if (assignment != null)
            {
                Lessons = new HashSet<LessonListDTO>();
                Tasks = new HashSet<TaskListDTO>();
                foreach (Lesson lesson in assignment.Lessons)
                    Lessons.Add(new LessonListDTO(lesson));
                foreach (Task task in assignment.Tasks)
                    Tasks.Add(new TaskListDTO(task));
            }
        }
        public ICollection<LessonListDTO> Lessons { get; }
        public ICollection<TaskListDTO> Tasks { get; }
        public int NewCourseId { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailTeacherDTO && Id == ((AssignmentDetailTeacherDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Assignment assignment)
        {
            bool finalOk = true;
            double maxScore = 0;
            HashSet<string> names = new HashSet<string>();
            HashSet<int> numbers = new HashSet<int>();
            if (assignment != null)
            {
                finalOk = IsFinal || !assignment.IsFinal;
                if (assignment.Tasks != null)
                {
                    foreach (Task task in assignment.Tasks)
                        maxScore += task.MaxScore;
                }
                if (assignment.Course != null && assignment.Course.Assignments != null)
                {
                    foreach (Assignment otherAssignment in assignment.Course.Assignments)
                    {
                        names.Add(otherAssignment.Name);
                        numbers.Add(otherAssignment.Number);
                    }
                }
                names.Remove(assignment.Name);
                numbers.Remove(assignment.Number);
            }
            return
                Name != null && Number > 0 && !names.Contains(Name) && !numbers.Contains(Number) &&
                MinGroupSize > 0 && MaxGroupSize > 0 && MinGroupSize <= MaxGroupSize &&
                MinRequiredScore >= 0 && MinRequiredScore <= maxScore && Weight >= 0 && finalOk;
        }
        public void Update(Assignment assignment)
        {
            assignment.Name = Name;
            assignment.Number = Number;
            assignment.Deadline = Deadline;
            assignment.IsFinal = IsFinal;
            assignment.Mandatory = Mandatory;
            assignment.MinGroupSize = MinGroupSize;
            assignment.MaxGroupSize = MaxGroupSize;
            assignment.MinRequiredScore = MinRequiredScore;
            assignment.Weight = Weight;
        }
        public Assignment Create()
        {
            Assignment assignment = new Assignment();
            Update(assignment);
            assignment.CourseId = NewCourseId;
            return assignment;
        }
    }

    public class AssignmentDetailStudentDTO : AssignmentDetailDTO
    {
        public AssignmentDetailStudentDTO(Assignment assignment, Student student) : base(assignment)
        {
            if (assignment != null && student != null)
            {
                Tasks = new HashSet<TaskListDTO>();
                foreach (GroupMembership membership in student.GroupMemberships)
                {
                    if (assignment.Equals(membership.Group.Lesson.Assignment))
                    {
                        Lesson = new LessonListDTO(membership.Group.Lesson);
                        Group = new GroupDetailStudentDTO(membership.Group);
                        if (membership.Group.IsFinal)
                        {
                            foreach (Task task in assignment.Tasks)
                                Tasks.Add(new TaskListDTO(task));
                        }
                        else
                        {
                            foreach (Evaluation evaluation in membership.Group.Evaluations)
                                Tasks.Add(new TaskDetailDTO(evaluation.Task, evaluation));
                        }
                        break;
                    }
                }
            }
        }
        public LessonListDTO Lesson { get; set; }
        public GroupDetailStudentDTO Group { get; set; }
        public ICollection<TaskListDTO> Tasks { get; }
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
