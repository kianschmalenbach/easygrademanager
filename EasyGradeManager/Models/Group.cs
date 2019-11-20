using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Group
    {
        public Group()
        {
            Password = new Random().Next(10000000, 99999999).ToString();
            Evaluations = new HashSet<Evaluation>();
            GroupMemberships = new HashSet<GroupMembership>();
            IsFinal = false;
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        public string Password { get; }
        public bool IsFinal { get; set; }
        [Required]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; }
        public virtual ICollection<GroupMembership> GroupMemberships { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Group && Id == ((Group)other).Id;
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
            if (group != null)
            {
                Tasks = new HashSet<TaskDetailDTO>();
                if (group.Lesson != null)
                    Lesson = new LessonListDTO(group.Lesson);
                if (group.Lesson.Assignment != null)
                    Assignment = new AssignmentListDTO(group.Lesson.Assignment);
                if (group.Lesson.Assignment.Course != null)
                    Course = new CourseListDTO(group.Lesson.Assignment.Course);
                foreach (Evaluation evaluation in group.Evaluations)
                    Tasks.Add(new TaskDetailDTO(evaluation.Task, evaluation));
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
    }

    public class GroupDetailStudentDTO : GroupListDTO
    {
        public GroupDetailStudentDTO(Group group, int membershipId) : base(group)
        {
            if (group != null)
            {
                Password = group.Password;
            }
            GroupMembershipId = membershipId;
        }
        public string Password { get; }
        public int GroupMembershipId { get; set; }
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
