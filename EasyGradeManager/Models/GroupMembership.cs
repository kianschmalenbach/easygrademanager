using System;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class GroupMembership
    {
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupMembership && Id == ((GroupMembership)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GroupMembershipDTO
    {
        public GroupMembershipDTO(GroupMembership membership)
        {
            if (membership != null)
            {
                Id = membership.Id;
                if (membership.Group != null)
                    Group = new GroupDetailStudentDTO(membership.Group);
            }
        }
        public int Id { get; set; }
        public GroupDetailStudentDTO Group { get; }
        public int NewLessonId { get; set; }
        public int NewAssignmentId { get; set; }
        public int NewGroupNumber { get; set; }
        public string NewGroupPassword { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupMembershipDTO && Id == ((GroupMembershipDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Student student, Group group, Assignment assignment)
        {
            if (student == null || assignment == null || assignment.Lessons == null || assignment.MembershipsFinal)
                return false;
            if (group != null && group.GroupMemberships != null && group.GroupMemberships.Count >= assignment.MaxGroupSize)
                return false;
            foreach (Lesson lesson in assignment.Lessons)
            {
                if (lesson.Groups != null)
                {
                    foreach (Group otherGroup in lesson.Groups)
                    {
                        if (otherGroup.GroupMemberships != null)
                        {
                            foreach (GroupMembership membership in otherGroup.GroupMemberships)
                            {
                                if (student.Equals(membership.Student))
                                    return false;
                            }
                        }
                    }
                }
            }
            return group == null || (NewGroupPassword != null && NewGroupPassword.Equals(group.Password));
        }
        public GroupMembership Create(Student student, Group group, Lesson lesson)
        {
            if (student == null)
                return null;
            GroupMembership membership = new GroupMembership();
            if (group == null)
            {
                if (lesson == null || lesson.Assignment == null)
                    return null;
                group = new Group
                {
                    LessonId = lesson.Id,
                    Number = lesson.Assignment.NextGroupNumber++,
                    Password = new Random().Next(10000000, 99999999).ToString()
                };
                membership.Group = group;
            }
            else
                membership.GroupId = group.Id;
            membership.StudentId = student.Id;
            return membership;
        }
    }
}
