using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

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
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Entity<Evaluation>()
            //    .HasKey(evaluation => new { evaluation.GroupId, evaluation.TaskId });
            //modelBuilder.Entity<GroupMembership>()
            //    .HasKey(membership => new { membership.GroupId, membership.StudentId });
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<GradingScheme> GradingSchemes { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Evaluation> Evaluations { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Role> Persons{ get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<GroupMembership> GroupMemberships { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Tutor> Tutors { get; set; }
    }
}
