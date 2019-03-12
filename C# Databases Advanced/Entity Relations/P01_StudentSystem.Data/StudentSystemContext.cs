namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {

        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.SqlConnect);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigStudentEntity(modelBuilder);

            ConfigCourseEntity(modelBuilder);

            ConfigResourceEntity(modelBuilder);

            ConfigHomeworkEntity(modelBuilder);

            ConfigStudentCourseEntity(modelBuilder);
        }

        private static void ConfigStudentCourseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
            .HasKey(sc => new { sc.CourseId, sc.StudentId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(s => s.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(sc => sc.CourseId);
        }

        private static void ConfigHomeworkEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Homework>(entity =>
            {
                entity
                    .HasKey(h => h.HomeworkId);

                entity
                    .Property(h => h.Content)
                    .IsRequired();

                entity
                    .HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions);

                entity
                    .HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions);
            });
        }

        private static void ConfigResourceEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity
                    .HasKey(r => r.ResourceId);

                entity
                    .Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsUnicode();

                entity
                    .Property(c => c.Url)
                    .IsRequired()
                    .IsUnicode(false);

                entity
                    .HasOne(r => r.Course)
                    .WithMany(c => c.Resources);
            });
        }

        private static void ConfigCourseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity
                    .HasKey(c => c.CourseId);

                entity
                    .Property(c => c.Name)
                    .HasMaxLength(80)
                    .IsUnicode();

                entity
                    .Property(c => c.Description)
                    .IsUnicode()
                    .IsRequired(false);
            });
        }

        private static void ConfigStudentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity
                    .HasKey(s => s.StudentId);

                entity
                    .Property(s => s.Name)
                    .HasMaxLength(100)
                    .IsUnicode();

                entity
                    .Property(s => s.PhoneNumber)
                    .HasColumnType("CHAR(10)")
                    .IsRequired(false);
            });
        }
    }
}