using System;
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
        public DateTime Deadline { get; set; }
        public double MinRequiredScore { get; set; }
        public bool Mandatory { get; set; }
        public double Weight { get; set; }
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public int NextGroupNumber { get; set; }
        public bool IsFinal { get; set; }
        public bool IsGraded { get; set; }
        public bool MembershipsFinal { get; set; }
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
        public double GetMaxScore()
        {
            if (Tasks == null || Tasks.Count == 0)
                return 0;
            double result = 0;
            foreach (Task task in Tasks)
                result += task.MaxScore;
            return result;
        }
        public double GetScore(Student student)
        {
            if (student == null || student.GroupMemberships == null)
                return 0;
            double score = 0;
            foreach (GroupMembership membership in student.GroupMemberships)
            {
                if (membership?.Group?.Lesson?.Assignment == null || membership.Group.Evaluations == null)
                    continue;
                if (Equals(membership.Group.Lesson.Assignment))
                {
                    foreach (Evaluation evaluation in membership.Group.Evaluations)
                        score += evaluation.Score;
                }
            }
            return score;
        }
        public bool HasPassed(Student student)
        {
            return GetScore(student) >= MinRequiredScore;
        }
        public string GetGrade(Student student)
        {
            if (student == null)
                return null;
            double score = GetScore(student);
            double maxScore = GetMaxScore();
            GradingScheme scheme = Course.GradingScheme;
            if (maxScore <= 0 || scheme == null || scheme.Grades == null || scheme.Grades.Count == 0)
                return null;
            double percentage = score / maxScore;
            if (percentage < 0 || percentage > 1)
                return null;
            return scheme.GetGrade(percentage);
        }
        public ICollection<Student> GetStudents(Tutor tutor)
        {
            ICollection<Student> students = new HashSet<Student>();
            if (Lessons != null)
            {
                foreach (Lesson lesson in Lessons)
                {
                    if (tutor != null && !tutor.Equals(lesson.Tutor))
                        continue;
                    foreach (Student student in lesson.GetStudents())
                        students.Add(student);
                }
            }
            return students;
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
                IsGraded = assignment.IsGraded;
                Mandatory = assignment.Mandatory;
                MaxGroupSize = assignment.MaxGroupSize;
                MinGroupSize = assignment.MinGroupSize;
                MinRequiredScore = assignment.MinRequiredScore;
                Name = assignment.Name;
                Number = assignment.Number;
                Weight = assignment.Weight;
                MembershipsFinal = assignment.MembershipsFinal;
            }
        }
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public double MinRequiredScore { get; set; }
        public bool Mandatory { get; set; }
        public double Weight { get; set; }
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public bool IsFinal { get; set; }
        public bool IsGraded { get; set; }
        public bool MembershipsFinal { get; set; }
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
        protected AssignmentDetailDTO(Assignment assignment, Tutor tutor) : base(assignment)
        {
            if (assignment != null)
            {
                Lessons = new HashSet<LessonListDTO>();
                if (assignment.Course != null)
                    Course = new CourseListDTO(assignment.Course);
                if (assignment.Lessons != null)
                {
                    foreach (Lesson lesson in assignment.Lessons)
                        Lessons.Add(new LessonListDTO(lesson, tutor));
                }
            }
        }
        public ICollection<LessonListDTO> Lessons { get; }
        public CourseListDTO Course { get; set; }
        public GroupMembershipDTO GroupMembership { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailDTO && Id == ((AssignmentDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class AssignmentDetailStudentDTO : AssignmentDetailDTO
    {
        public AssignmentDetailStudentDTO(Assignment assignment, Student student, Tutor tutor) : base(assignment, tutor)
        {
            if (assignment != null && student != null)
            {
                Tasks = new HashSet<TaskListDTO>();
                HashSet<int> taskNumbers = new HashSet<int>();
                foreach (GroupMembership membership in student.GroupMemberships)
                {
                    if (assignment.Equals(membership.Group.Lesson.Assignment))
                    {
                        Lesson = new LessonListDTO(membership.Group.Lesson, null);
                        GroupMembership = new GroupMembershipDTO(membership);
                        foreach (Evaluation evaluation in membership.Group.Evaluations)
                        {
                            taskNumbers.Add(evaluation.Task.Number);
                            Tasks.Add(new TaskDetailDTO(evaluation.Task, evaluation));
                        }
                        break;
                    }
                }
                if (assignment.Tasks != null)
                {
                    foreach (Task task in assignment.Tasks)
                    {
                        if (!taskNumbers.Contains(task.Number))
                            Tasks.Add(new TaskListDTO(task));
                    }
                }
                Result = new AssignmentResult(student, assignment, false);
            }
        }
        public LessonListDTO Lesson { get; set; }
        public ICollection<TaskListDTO> Tasks { get; }
        public AssignmentResult Result { get; }
        public override bool Equals(object other)
        {
            return other != null && other is AssignmentDetailStudentDTO && Id == ((AssignmentDetailStudentDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class AssignmentDetailTeacherDTO : AssignmentDetailStudentDTO
    {
        public AssignmentDetailTeacherDTO(Assignment assignment, Student student, Tutor tutor, Teacher teacher) : base(assignment, student, tutor)
        {
            Results = new HashSet<AssignmentResult>();
            if (assignment != null)
            {
                if (Tasks.Count == 0)
                    foreach (Task task in assignment.Tasks)
                        Tasks.Add(new TaskListDTO(task));
                if (teacher != null || tutor != null)
                {
                    ICollection<Student> students = assignment.GetStudents(teacher != null ? null : tutor);
                    foreach (Student otherStudent in students)
                        Results.Add(new AssignmentResult(otherStudent, assignment, true));
                }
            }
        }
        public int NewCourseId { get; set; }
        public ICollection<AssignmentResult> Results { get; }
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
            bool membershipsFinalOk = true;
            bool gradedOk = true;
            double maxScore = 0;
            HashSet<string> names = new HashSet<string>();
            HashSet<int> numbers = new HashSet<int>();
            if (assignment != null)
            {
                finalOk = (!IsFinal && !assignment.IsFinal) || (IsFinal && MembershipsFinal);
                membershipsFinalOk = (!assignment.IsFinal && !assignment.MembershipsFinal) || MembershipsFinal;
                gradedOk = !IsFinal || (IsGraded == assignment.IsGraded);
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
                MinRequiredScore >= 0 && MinRequiredScore <= maxScore && Weight >= 0 && finalOk &&
                membershipsFinalOk && gradedOk;
        }
        public void Update(Assignment assignment)
        {
            assignment.Name = Name;
            assignment.Number = Number;
            assignment.Deadline = Deadline;
            assignment.IsGraded = IsGraded;
            assignment.IsFinal = IsFinal;
            assignment.Mandatory = Mandatory;
            assignment.MembershipsFinal = MembershipsFinal;
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
            assignment.NextGroupNumber = 1;
            return assignment;
        }

    }

    public class AssignmentResult
    {
        internal AssignmentResult(Student student, Assignment assignment, bool setStudent)
        {
            if (student != null && assignment != null)
            {
                if (setStudent)
                    Student = new UserListDTO(student.User);
                Id = student.Id;
                Score = assignment.GetScore(student);
                HasPassed = assignment.HasPassed(student);
                Grade = assignment.GetGrade(student);
            }
        }
        public int Id { get; }
        public UserListDTO Student { get; }
        public double Score { get; }
        public bool HasPassed { get; }
        public string Grade { get; }
    }
}
