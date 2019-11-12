using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class Lesson
    {
        public Lesson()
        {
            Groups = new HashSet<Group>();
            DerivedLessons = new HashSet<Lesson>();
        }
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
        [Required]
        public int TutorId { get; set; }
        public virtual Tutor Tutor { get; set; }
        public int? DerivedFromId { get; set; }
        public virtual Lesson DerivedFrom { get; set; }
        public virtual ICollection<Lesson> DerivedLessons { get; }
        public virtual ICollection<Group> Groups { get; }
        public override bool Equals(object other)
        {
            return other != null && other is Lesson && Id == ((Lesson)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class LessonListDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public UserListDTO Tutor { get; set; }
        public int? DerivedFromId { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is LessonListDTO && Id == ((LessonListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class LessonDetailDTO : LessonListDTO
    {
        public LessonDetailDTO()
        {
            Groups = new HashSet<GroupDetailDTO>();
        }
        public ICollection<GroupDetailDTO> Groups { get; }
        public override bool Equals(object other)
        {
            return other != null && other is LessonDetailDTO && Id == ((LessonDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
