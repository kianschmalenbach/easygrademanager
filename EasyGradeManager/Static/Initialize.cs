using EasyGradeManager.Models;
using System;
using System.Linq;

namespace EasyGradeManager.Static
{
    public static class Initialize
    {
        private static readonly EasyGradeManagerContext db = new EasyGradeManagerContext();
        public static void InitializeDatabase()
        {
            if (db.Users.Count() > 0)
                return;
            InitializeUsers();
            InitializeRoles();
            InitializeGradingSchemes();
            InitializeCourses();
            InitializeAssignments();
            InitializeTasks();
            InitializeLessons();
            InitializeGroups();
            InitializeGroupMemberships();
            InitializeEvaluations();
        }

        private static void InitializeUsers()
        {
            db.Users.Add(new User()
            {
                Identifier = "acooper",
                Name = "Dr. Alice Cooper",
                Email = "alice.cooper@example.org",
                Password = SecurePasswordHasher.Hash("teaching123")
            });
            db.Users.Add(new User()
            {
                Identifier = "smiller",
                Name = "Sarah Miller B.Sc.",
                Email = "sarah.miller@example.org",
                Password = SecurePasswordHasher.Hash("tutor456")
            });
            db.Users.Add(new User()
            {
                Identifier = "rsmith",
                Name = "Roger Smith M.Sc.",
                Email = "roger.smith@example.org",
                Password = SecurePasswordHasher.Hash("rogibaby")
            });
            db.Users.Add(new User()
            {
                Identifier = "smeyer",
                Name = "Sandra Meyer",
                Email = "sandra.meyer@example.org",
                Password = SecurePasswordHasher.Hash("cutieputie")
            });
            db.Users.Add(new User()
            {
                Identifier = "agonzales",
                Name = "Alex Gonzales",
                Email = "alex.gonzales@example.org",
                Password = SecurePasswordHasher.Hash("robinhood")
            });
            db.Users.Add(new User()
            {
                Identifier = "amay",
                Name = "Anna May",
                Email = "anna.may@example.org",
                Password = SecurePasswordHasher.Hash("paradise35!")
            });
            db.Users.Add(new User()
            {
                Identifier = "dsilva",
                Name = "Diego Silva",
                Email = "diego.silva@example.org",
                Password = SecurePasswordHasher.Hash("panamarama")
            });
            db.Users.Add(new User()
            {
                Identifier = "mfoucault",
                Name = "Marie Foucault",
                Email = "marie.foucault@example.org",
                Password = SecurePasswordHasher.Hash("bijoux123")
            });
            db.Users.Add(new User()
            {
                Identifier = "ksörensen",
                Name = "Katja Sörensen",
                Email = "katja.soerensen@example.org",
                Password = SecurePasswordHasher.Hash("smörebröd")
            });
            db.Users.Add(new User()
            {
                Identifier = "pdobrowski",
                Name = "Pietr Dobrowski",
                Email = "pietr.dobrowski@example.org",
                Password = SecurePasswordHasher.Hash("butterfly")
            });
            db.SaveChanges();
        }

        private static void InitializeRoles()
        {
            db.Teachers.Add(new Teacher()
            {
                UserId = 1
            });
            db.Tutors.Add(new Tutor()
            {
                UserId = 1
            });
            db.Tutors.Add(new Tutor()
            {
                UserId = 2
            });
            db.Tutors.Add(new Tutor()
            {
                UserId = 3
            });
            db.Students.Add(new Student()
            {
                UserId = 4
            });
            db.Students.Add(new Student()
            {
                UserId = 5
            });
            db.Students.Add(new Student()
            {
                UserId = 6
            });
            db.Students.Add(new Student()
            {
                UserId = 7
            });
            db.Students.Add(new Student()
            {
                UserId = 8
            });
            db.Students.Add(new Student()
            {
                UserId = 9
            });
            db.Students.Add(new Student()
            {
                UserId = 10
            });
            db.Students.Add(new Student()
            {
                UserId = 1
            });
            db.Students.Add(new Student()
            {
                UserId = 2
            });
            db.Students.Add(new Student()
            {
                UserId = 3
            });

            db.SaveChanges();
        }

        private static void InitializeGradingSchemes()
        {
            var gradingScheme = new GradingScheme()
            {
                Name = "French"
            };
            db.GradingSchemes.Add(gradingScheme);
            db.SaveChanges();
            for (double i = 0; i <= 20; ++i)
            {
                db.Grades.Add(new Grade()
                {
                    GradingSchemeId = 1,
                    MinPercentage = i / 20,
                    Name = i.ToString()
                });
                db.SaveChanges();
            }
            gradingScheme = new GradingScheme()
            {
                Name = "American"
            };
            db.GradingSchemes.Add(gradingScheme);
            db.SaveChanges();
            string[] grades = new string[] { "D", "C", "B", "A" };
            for (double i = 0; i < 4; ++i)
            {
                db.Grades.Add(new Grade()
                {
                    GradingSchemeId = 2,
                    MinPercentage = i / 4,
                    Name = grades[(int)i]
                });
                db.SaveChanges();
            }
        }

        private static void InitializeCourses()
        {
            db.Courses.Add(new Course()
            {
                Name = "Software Engineering",
                Term = "Winter Term 2019/20",
                TeacherId = 1,
                Archived = false,
                GradingSchemeId = 1,
                MinRequiredAssignments = 3,
                MinRequiredScore = 50
            });
            db.SaveChanges();
        }

        private static void InitializeAssignments()
        {
            db.Assignments.Add(new Assignment()
            {
                CourseId = 1,
                Deadline = new DateTime(2019, 10, 25),
                IsFinal = true,
                IsGraded = false,
                Mandatory = false,
                MinRequiredScore = 10,
                MinGroupSize = 2,
                MaxGroupSize = 3,
                Name = "Sheet 1",
                Number = 1,
                Weight = 0.2,
                NextGroupNumber = 4
            });
            db.SaveChanges();
            db.Assignments.Add(new Assignment()
            {
                CourseId = 1,
                Deadline = new DateTime(2019, 11, 01),
                IsFinal = true,
                IsGraded = false,
                Mandatory = false,
                MinRequiredScore = 10,
                MinGroupSize = 2,
                MaxGroupSize = 3,
                Name = "Sheet 2",
                Number = 2,
                Weight = 0.2,
                NextGroupNumber = 4
            });
            db.SaveChanges();
            db.Assignments.Add(new Assignment()
            {
                CourseId = 1,
                Deadline = new DateTime(2019, 11, 08),
                IsFinal = true,
                IsGraded = false,
                Mandatory = false,
                MinRequiredScore = 10,
                MinGroupSize = 2,
                MaxGroupSize = 3,
                Name = "Sheet 3",
                Number = 3,
                Weight = 0.2,
                NextGroupNumber = 4
            });
            db.SaveChanges();
            db.Assignments.Add(new Assignment()
            {
                CourseId = 1,
                Deadline = new DateTime(2019, 11, 22),
                IsFinal = false,
                IsGraded = true,
                Mandatory = true,
                MinRequiredScore = 40,
                MinGroupSize = 1,
                MaxGroupSize = 1,
                Name = "Exam",
                Number = 4,
                Weight = 0.4,
                NextGroupNumber = 8
            });
            db.SaveChanges();
        }

        private static void InitializeTasks()
        {
            db.Tasks.Add(new Task()
            {
                Number = 1,
                Name = "Task 1",
                MaxScore = 8,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 2,
                Name = "Task 2",
                MaxScore = 6,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 3,
                Name = "Task 3",
                MaxScore = 6,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 1,
                Name = "Task 1",
                MaxScore = 7,
                AssignmentId = 2
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 2,
                Name = "Task 2",
                MaxScore = 7,
                AssignmentId = 2
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 3,
                Name = "Task 3",
                MaxScore = 6,
                AssignmentId = 2
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 1,
                Name = "Task 1",
                MaxScore = 5,
                AssignmentId = 3
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 2,
                Name = "Task 2",
                MaxScore = 6,
                AssignmentId = 3
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 3,
                Name = "Task 3",
                MaxScore = 9,
                AssignmentId = 3
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 1,
                Name = "Question 1",
                MaxScore = 20,
                AssignmentId = 4
            });
            db.SaveChanges();
            db.Tasks.Add(new Task()
            {
                Number = 2,
                Name = "Question 2",
                MaxScore = 40,
                AssignmentId = 4
            });
        }

        private static void InitializeLessons()
        {
            db.Lessons.Add(new Lesson()
            {
                Number = 1,
                Date = new DateTime(2019, 10, 28, 10, 00, 00),
                TutorId = 3,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 2,
                Date = new DateTime(2019, 10, 28, 11, 00, 00),
                TutorId = 3,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 3,
                Date = new DateTime(2019, 10, 29, 14, 00, 00),
                TutorId = 4,
                AssignmentId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 1,
                Date = new DateTime(2019, 11, 05, 10, 00, 00),
                TutorId = 3,
                AssignmentId = 2,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 2,
                Date = new DateTime(2019, 11, 05, 11, 00, 00),
                TutorId = 3,
                AssignmentId = 2,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 3,
                Date = new DateTime(2019, 11, 06, 14, 00, 00),
                TutorId = 4,
                AssignmentId = 2,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 1,
                Date = new DateTime(2019, 11, 12, 10, 00, 00),
                TutorId = 3,
                AssignmentId = 3,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 2,
                Date = new DateTime(2019, 11, 12, 11, 00, 00),
                TutorId = 3,
                AssignmentId = 3,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 3,
                Date = new DateTime(2019, 11, 13, 14, 00, 00),
                TutorId = 4,
                AssignmentId = 3,
                DerivedFromId = 1
            });
            db.SaveChanges();
            db.Lessons.Add(new Lesson()
            {
                Number = 1,
                Date = new DateTime(2019, 11, 22, 09, 00, 00),
                TutorId = 2,
                AssignmentId = 4
            });
            db.SaveChanges();
        }

        private static void InitializeGroups()
        {
            string password1 = new Random().Next(10000000, 99999999).ToString();
            string password2 = new Random().Next(10000000, 99999999).ToString();
            string password3 = new Random().Next(10000000, 99999999).ToString();

            db.Groups.Add(new Group()
            {
                Number = 1,
                LessonId = 1,
                Password = password1,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 2,
                LessonId = 1,
                Password = password2,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 3,
                LessonId = 3,
                Password = password3,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 1,
                LessonId = 4,
                Password = password1,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 2,
                LessonId = 4,
                Password = password2,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 3,
                LessonId = 6,
                Password = password3,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 1,
                LessonId = 7,
                Password = password1,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 2,
                LessonId = 8,
                Password = password2,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 3,
                LessonId = 9,
                Password = password3,
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 1,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString(),
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 2,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString(),
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 3,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString(),
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 4,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString(),
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 5,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString(),
                IsFinal = true
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 6,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString()
            });
            db.SaveChanges();
            db.Groups.Add(new Group()
            {
                Number = 7,
                LessonId = 10,
                Password = new Random().Next(10000000, 99999999).ToString()
            });
            db.SaveChanges();
        }

        private static void InitializeGroupMemberships()
        {
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 1,
                StudentId = 5
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 1,
                StudentId = 6
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 2,
                StudentId = 7
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 2,
                StudentId = 8
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 3,
                StudentId = 9
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 3,
                StudentId = 10
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 3,
                StudentId = 11
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 4,
                StudentId = 5
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 4,
                StudentId = 6
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 5,
                StudentId = 7
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 5,
                StudentId = 8
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 6,
                StudentId = 9
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 6,
                StudentId = 10
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 6,
                StudentId = 11
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 7,
                StudentId = 5
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 7,
                StudentId = 6
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 8,
                StudentId = 7
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 8,
                StudentId = 8
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 8,
                StudentId = 9
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 9,
                StudentId = 10
            });
            db.SaveChanges();
            db.GroupMemberships.Add(new GroupMembership()
            {
                GroupId = 9,
                StudentId = 11
            });
            db.SaveChanges();
            for (int i = 5; i <= 11; ++i)
            {
                db.GroupMemberships.Add(new GroupMembership()
                {
                    GroupId = i + 5,
                    StudentId = i
                });
                db.SaveChanges();
            }
        }

        private static void InitializeEvaluations()
        {
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 1,
                TaskId = 1,
                Score = 5
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 2,
                TaskId = 1,
                Score = 7
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 3,
                TaskId = 1,
                Score = 3
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 1,
                TaskId = 2,
                Score = 3
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 2,
                TaskId = 2,
                Score = 6
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 3,
                TaskId = 2,
                Score = 0
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 1,
                TaskId = 3,
                Score = 4
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 2,
                TaskId = 3,
                Score = 6
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 3,
                TaskId = 3,
                Score = 2
            });
            db.SaveChanges();

            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 4,
                TaskId = 4,
                Score = 5
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 4,
                TaskId = 5,
                Score = 7
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 4,
                TaskId = 6,
                Score = 3
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 5,
                TaskId = 4,
                Score = 5
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 5,
                TaskId = 5,
                Score = 7
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 5,
                TaskId = 6,
                Score = 3
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 6,
                TaskId = 4,
                Score = 6
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 6,
                TaskId = 5,
                Score = 4
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 6,
                TaskId = 6,
                Score = 5
            });
            db.SaveChanges();

            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 7,
                TaskId = 7,
                Score = 4
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 7,
                TaskId = 8,
                Score = 5
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 7,
                TaskId = 9,
                Score = 8
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 8,
                TaskId = 7,
                Score = 5
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 8,
                TaskId = 8,
                Score = 6
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 8,
                TaskId = 9,
                Score = 7
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 9,
                TaskId = 7,
                Score = 3
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 9,
                TaskId = 8,
                Score = 2
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 9,
                TaskId = 9,
                Score = 7
            });
            db.SaveChanges();

            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 10,
                TaskId = 10,
                Score = 18
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 10,
                TaskId = 11,
                Score = 35
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 11,
                TaskId = 10,
                Score = 9
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 11,
                TaskId = 11,
                Score = 14
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 12,
                TaskId = 10,
                Score = 14
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 12,
                TaskId = 11,
                Score = 38
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 13,
                TaskId = 10,
                Score = 17
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 13,
                TaskId = 11,
                Score = 24
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 14,
                TaskId = 10,
                Score = 0
            });
            db.SaveChanges();
            db.Evaluations.Add(new Evaluation()
            {
                GroupId = 14,
                TaskId = 11,
                Score = 0
            });
            db.SaveChanges();
        }
    }
}
