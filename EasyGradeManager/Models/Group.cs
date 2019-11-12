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
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsFinal { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupListDTO && Id == ((GroupListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
    public class GroupDetailDTO : GroupListDTO
    {
        public GroupDetailDTO()
        {
            Students = new HashSet<UserListDTO>();
        }
        public string Password { get; }
        public ICollection<UserListDTO> Students { get; }
        public override bool Equals(object other)
        {
            return other != null && other is GroupDetailDTO && Id == ((GroupDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
