using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Group
    {
        public Group()
        {
            this.Password = new Random().Next(10000000, 99999999).ToString();
            this.Evaluations = new HashSet<Evaluation>();
            this.GroupMemberships = new HashSet<GroupMembership>();
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        public string Password { get; set; }
        [Required]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<GroupMembership> GroupMemberships { get; set; }
    }
}
