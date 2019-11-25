using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Group
    {
        public Group()
        {
            Evaluations = new HashSet<Evaluation>();
            GroupMemberships = new HashSet<GroupMembership>();
            IsFinal = false;
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        public string Password { get; set; }
        public bool IsFinal { get; set; }
        [Required]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; }
        public virtual ICollection<GroupMembership> GroupMemberships { get; }
        public override bool Equals(object other)
        {
            return
                other != null && other is Group && Id == ((Group)other).Id &&
                (Id != 0 || (LessonId == ((Group)other).LessonId && Number == ((Group)other).Number));
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GroupListDTO
    {
        public GroupListDTO(Group group)
        {
            if (group != null)
            {
                Id = group.Id;
                Number = group.Number;
                IsFinal = group.IsFinal;
                Students = new HashSet<UserListDTO>();
                foreach (GroupMembership membership in group.GroupMemberships)
                    Students.Add(new UserListDTO(membership.Student.User));
            }
        }
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsFinal { get; set; }
        public ICollection<UserListDTO> Students { get; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupListDTO && Id == ((GroupListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GroupDetailTeacherDTO : GroupListDTO
    {
        public GroupDetailTeacherDTO(Group group) : base(group)
        {
            Tasks = new HashSet<TaskDetailDTO>();
            HashSet<int> taskNumbers = new HashSet<int>();
            if (group != null)
            {
                if (group.Lesson != null)
                    Lesson = new LessonListDTO(group.Lesson, null);
                if (group.Lesson.Assignment != null)
                    Assignment = new AssignmentListDTO(group.Lesson.Assignment);
                if (group.Lesson.Assignment.Course != null)
                    Course = new CourseListDTO(group.Lesson.Assignment.Course);
                foreach (Evaluation evaluation in group.Evaluations)
                {
                    taskNumbers.Add(evaluation.Task.Number);
                    Tasks.Add(new TaskDetailDTO(evaluation.Task, evaluation));
                }
                foreach (Task task in group.Lesson.Assignment.Tasks)
                {
                    if (!taskNumbers.Contains(task.Number))
                        Tasks.Add(new TaskDetailDTO(task, null));
                }
            }
        }
        public CourseListDTO Course { get; set; }
        public AssignmentListDTO Assignment { get; set; }
        public LessonListDTO Lesson { get; set; }
        public ICollection<TaskDetailDTO> Tasks { get; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupDetailTeacherDTO && Id == ((GroupDetailTeacherDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Group group, bool isTeacher)
        {
            if (group == null || group.Lesson == null || group.Lesson.Assignment == null ||
                group.Lesson.Assignment.Tasks == null || group.Evaluations == null)
                return false;
            if ((group.IsFinal && (!isTeacher || group.Lesson.Assignment.IsFinal || !IsFinal)) || (!group.IsFinal && isTeacher))
                return false;
            foreach (TaskDetailDTO taskDTO in Tasks)
            {
                Task task = null;
                foreach (Task otherTask in group.Lesson.Assignment.Tasks)
                {
                    if (taskDTO.Id == otherTask.Id)
                    {
                        task = otherTask;
                        break;
                    }
                }
                if (task == null || taskDTO.Score < 0 || taskDTO.Score > task.MaxScore)
                    return false;
            }
            return true;

        }
        public void Update(Group group)
        {
            group.IsFinal = IsFinal;
            foreach (TaskDetailDTO taskDTO in Tasks)
            {
                bool set = false;
                foreach (Evaluation evaluation in group.Evaluations)
                {
                    if (taskDTO.Id == evaluation.TaskId)
                    {
                        evaluation.Score = taskDTO.Score;
                        set = true;
                        break;
                    }
                }
                if (!set)
                {
                    group.Evaluations.Add(new Evaluation()
                    {
                        GroupId = group.Id,
                        TaskId = taskDTO.Id,
                        Score = taskDTO.Score
                    });
                }
            }
        }
    }

    public class GroupDetailStudentDTO : GroupListDTO
    {
        public GroupDetailStudentDTO(Group group) : base(group)
        {
            if (group != null)
            {
                Password = group.Password;
            }
        }
        public string Password { get; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupDetailStudentDTO && Id == ((GroupDetailStudentDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
