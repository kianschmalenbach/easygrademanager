using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Lesson
    {
        public Lesson()
        {
            this.Groups = new HashSet<Group>();
            this.DerivedLessons = new HashSet<Lesson>();
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        public DateTime[] Dates { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
        [Required]
        public int TutorId { get; set; }
        public virtual Tutor Tutor { get; set; }
        public int? DerivedFromId { get; set; }
        public virtual Lesson DerivedFrom { get; set; }
        public virtual ICollection<Lesson> DerivedLessons { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
