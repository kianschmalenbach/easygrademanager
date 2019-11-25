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
            return
                other != null && other is Lesson && Id == ((Lesson)other).Id &&
                (Id != 0 || (AssignmentId == ((Lesson)other).AssignmentId && Number == ((Lesson)other).Number));
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public ICollection<Student> GetStudents()
        {
            ICollection<Student> students = new HashSet<Student>();
            if (Groups != null)
            {
                foreach (Group group in Groups)
                {
                    if (group.GroupMemberships != null)
                    {
                        foreach (GroupMembership membership in group.GroupMemberships)
                        {
                            if (membership.Student != null)
                                students.Add(membership.Student);
                        }
                    }
                }
            }
            return students;
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
        public string NewTutorIdentifier { get; set; }
        public ICollection<GroupListDTO> Groups { get; }
        public LessonListDTO DerivedFrom { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is LessonDetailDTO && Id == ((LessonDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(Lesson lesson, Assignment assignment, Tutor tutor)
        {
            if (tutor == null)
                return false;
            HashSet<int> numbers = new HashSet<int>();
            if (lesson != null && lesson.Assignment != null && lesson.Assignment.Lessons != null)
            {
                foreach (Lesson otherLesson in lesson.Assignment.Lessons)
                    numbers.Add(otherLesson.Number);
                numbers.Remove(lesson.Number);
                return NewAssignmentId == 0 && !numbers.Contains(Number);
            }
            if (lesson == null)
            {
                if (assignment == null || assignment.Course == null || assignment.Id != NewAssignmentId ||
                    assignment.Lessons == null)
                    return false;
                foreach (Lesson otherLesson in assignment.Lessons)
                    numbers.Add(otherLesson.Number);
                return !numbers.Contains(Number);
            }
            return false;
        }
        public void Update(Lesson lesson, Tutor tutor)
        {
            lesson.Number = Number;
            lesson.Date = Date;
            if (tutor != null)
                lesson.TutorId = tutor.Id;
        }
        public Lesson Create(Tutor tutor)
        {
            Lesson lesson = new Lesson();
            Update(lesson, tutor);
            lesson.AssignmentId = NewAssignmentId;
            return lesson;
        }
    }
}
