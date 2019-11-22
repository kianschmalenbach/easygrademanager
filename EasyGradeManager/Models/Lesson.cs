using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

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
        public LessonListDTO(Lesson lesson, Tutor tutor)
        {
            if (lesson != null)
            {
                Id = lesson.Id;
                Number = lesson.Number;
                Date = lesson.Date;
                IsOwnLesson = true;
                if (lesson.Tutor != null)
                {
                    Tutor = new UserListDTO(lesson.Tutor.User);
                    if (tutor != null)
                        IsOwnLesson = lesson.Tutor.Equals(tutor);
                }
            }
        }
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public UserListDTO Tutor { get; set; }
        public bool IsOwnLesson { get; }
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
        public LessonDetailDTO(Lesson lesson) : base(lesson, null)
        {
            if (lesson != null)
            {
                if (lesson.Assignment != null)
                    Assignment = new AssignmentListDTO(lesson.Assignment);
                if (lesson.Assignment.Course != null)
                    Course = new CourseListDTO(lesson.Assignment.Course);
                Groups = new HashSet<GroupListDTO>();
                foreach (Group group in lesson.Groups)
                    Groups.Add(new GroupListDTO(group));
                if (lesson.DerivedFrom != null)
                    DerivedFrom = new LessonListDTO(lesson.DerivedFrom, null);
            }
        }
        public CourseListDTO Course { get; set; }
        public AssignmentListDTO Assignment { get; set; }
        public int NewAssignmentId { get; set; }
        public int NewTutorId { get; set; }
        public ICollection<GroupListDTO> Groups { get; }
        public LessonListDTO DerivedFrom { get; set; }
        public int NewDerivedFromId { get; set; }
        public int NewDayOffset { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is LessonDetailDTO && Id == ((LessonDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Lesson lesson, DbSet<Lesson> allLessons, Assignment assignment, DbSet<Tutor> allTutors)
        {
            if (NewTutorId != 0 && (NewDerivedFromId != 0 || allTutors == null || allTutors.Find(NewTutorId) == null))
                return false;
            HashSet<int> numbers = new HashSet<int>();
            if (lesson != null && lesson.Assignment != null && lesson.Assignment.Lessons != null)
            {
                foreach (Lesson otherLesson in lesson.Assignment.Lessons)
                    numbers.Add(otherLesson.Number);
                numbers.Remove(lesson.Number);
                return NewDerivedFromId == 0 && NewAssignmentId == 0 && !numbers.Contains(Number);
            }
            if (lesson == null)
            {
                if (allLessons == null || assignment == null || assignment.Course == null ||
                    assignment.Id != NewAssignmentId || assignment.Lessons == null)
                    return false;
                foreach (Lesson otherLesson in assignment.Lessons)
                    numbers.Add(otherLesson.Number);
                if (NewDerivedFromId != 0)
                {
                    Lesson derivedFrom = allLessons.Find(NewDerivedFromId);
                    if (derivedFrom == null || derivedFrom.Tutor == null || derivedFrom.Assignment == null ||
                        derivedFrom.Assignment.Course == null || !assignment.Course.Equals(derivedFrom.Assignment.Course))
                        return false;
                    return !numbers.Contains(derivedFrom.Number) && NewTutorId == 0;
                }
                return !numbers.Contains(Number);
            }
            return false;
        }
        public void Update(Lesson lesson, Lesson derivedFrom)
        {
            if (derivedFrom != null)
            {
                lesson.DerivedFrom = derivedFrom;
                lesson.Number = derivedFrom.Number;
                lesson.Date = derivedFrom.Date.AddDays(NewDayOffset);
                lesson.TutorId = derivedFrom.TutorId;
            }
            else
            {
                lesson.Number = Number;
                lesson.Date = Date;
                if (NewTutorId != 0)
                    lesson.TutorId = NewTutorId;
            }
        }
        public Lesson Create(DbSet<Lesson> allLessons)
        {
            Lesson lesson = new Lesson();
            if (allLessons == null)
                return null;
            if (NewDerivedFromId != 0)
            {
                Lesson derivedFrom = allLessons.Find(NewDerivedFromId);
                if (derivedFrom == null)
                    return null;
                Update(lesson, derivedFrom);
            }
            else
            {
                Update(lesson, null);
            }
            lesson.AssignmentId = NewAssignmentId;
            return lesson;
        }
    }
}
