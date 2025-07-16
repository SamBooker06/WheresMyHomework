using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public required DbSet<School> Schools { get; init; }

    public required DbSet<SchoolClass> Classes { get; init; }

    public required DbSet<Subject> Subjects { get; init; }

    public required DbSet<HomeworkTask> HomeworkTasks { get; init; }
    public required DbSet<StudentHomeworkTask> StudentHomeworkTasks { get; init; }
    public required DbSet<Todo> Todos { get; set; }

    public required DbSet<Message> Messages { get; init; }
    public required DbSet<Tag> Tags { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Student>("Student")
            .HasValue<Teacher>("Teacher")
            .HasValue<Admin>("Admin")
            .IsComplete();

        builder.Entity<SchoolClass>()
            .HasOne(cls => cls.Teacher)
            .WithMany(teacher => teacher.Classes)
            .HasForeignKey(cls => cls.TeacherId);

        // Ensures todos are deleted when a student homework task is deleted
        builder.Entity<StudentHomeworkTask>()
            .HasMany(s => s.Todos)
            .WithOne(t => t.StudentHomeworkTask)
            .OnDelete(DeleteBehavior.Cascade);

        // Map many-to-many relationship of students and classes
        builder.Entity<SchoolClass>()
            .HasMany(schoolClass => schoolClass.Students)
            .WithMany(student => student.SchoolClasses);

        builder.Entity<HomeworkTask>()
            .HasMany(task => task.StudentHomeworkTasks)
            .WithOne(studentTask => studentTask.HomeworkTask)
            .OnDelete(DeleteBehavior.Cascade); // Ensure referential integrity
        
        builder.Entity<Tag>()
            .HasMany(tag => tag.StudentHomeworkTasks)
            .WithMany(task => task.Tags);

        // Create a composite key composed of the tag name and the student id
        builder.Entity<Tag>()
            .HasKey(tag => new { tag.Name, tag.StudentId });

        builder.Entity<Student>()
            .HasMany(student => student.TaskTags)
            .WithOne(tag => tag.Student)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

