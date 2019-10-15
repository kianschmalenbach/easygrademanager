using System.Data.Entity;

namespace EasyGradeManager.Models
{
    public class EasyGradeManagerContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public EasyGradeManagerContext() : base("name=EasyGradeManagerContext")
        {
        }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Assignment> Assignments { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Evaluation> Evaluations { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.EvaluationCriterion> EvaluationCriterions { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Lesson> Lessons { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Person> People { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Score> Scores { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Task> Tasks { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Teacher> Teachers { get; set; }

        public System.Data.Entity.DbSet<EasyGradeManager.Models.Tutor> Tutors { get; set; }
    }
}
