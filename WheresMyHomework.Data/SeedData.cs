using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data;

public static class SeedData
{
    private static readonly string[] RolesNames =
    {
        "Admin",
        "Teacher",
        "Student"
    };

    private const string AdminUserId = "Admin";
    private const string AdminPassword = "Admin$123";
    
    private const string TeacherUserId = "Teacher";
    private const string TeacherPassword = "Teacher$123";
    
    private const string StudentUserId = "Student";
    private const string StudentPassword = "Student$123";

    private static async Task InitialiseRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in RolesNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task<School> SeedSchoolAsync(ApplicationDbContext context)
    {
        var foundSchool = await context.Schools.FindAsync(1);
        if (foundSchool is not null) return foundSchool;

        var school = new School
        {
            Name = "School",
        };

        await context.Schools.AddAsync(school);
        return school;
    }

    private static async Task SeedTestUsers(UserManager<ApplicationUser> userManager, School school)
    {
        if (await userManager.FindByIdAsync(AdminUserId) is null)
        {
            var admin = new Admin
            {
                Id = AdminUserId,
                FirstName = "Admin",
                LastName = "Joe",
                Email = "admin@gmail.com",
                School = school,
                Title = PersonTitle.Mr,
                UserName = "Admin",
                EmailConfirmed = true,
            };
            var res = await userManager.CreateAsync(admin, AdminPassword);
            Debug.Assert(res.Succeeded);

            await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (await userManager.FindByIdAsync(TeacherUserId) is null)
        {
            var teacher = new Teacher
            {
                Id = TeacherUserId,
                FirstName = "Teacher",
                LastName = "Jane",
                Email = "teacher@gmail.com",
                School = school,
                Title = PersonTitle.Mrs,
                UserName = "Teacher",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(teacher, TeacherPassword);
            Debug.Assert(res.Succeeded);
            
            await userManager.AddToRoleAsync(teacher, "Teacher");
        }

        if (await userManager.FindByIdAsync(StudentUserId) is null)
        {
            var student = new Student
            {
                Id = StudentUserId,
                FirstName = "Student",
                LastName = "Steve",
                Email = "student@gmail.com",
                School = school,
                Title = PersonTitle.Miss,
                UserName = "Student",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(student, StudentPassword);
            Debug.Assert(res.Succeeded);
            
            await userManager.AddToRoleAsync(student, "Student");

        }
    }

    private static async Task SeedHomeworkTask(ApplicationDbContext context, SchoolClass schoolClass)
    {
        if (await context.HomeworkTasks.FindAsync(1) is not null) return;

        var student = (await context.Users.FindAsync(StudentUserId)) as Student;
        Debug.Assert(student != null);

        var homeworkTask = new HomeworkTask
        {
            Title = "Homework",
            Description = "Homework",
            DueDate = DateTime.Today + TimeSpan.FromDays(30),
            Class = schoolClass,
            
        };

        await context.HomeworkTasks.AddAsync(homeworkTask);

        var studentHomeworkTask = new StudentHomeworkTask
        {
            Id = 0,
            Student = student,
            HomeworkTask = homeworkTask,
            Notes = "Complete this later",
            Priority = Priority.Low,
        };

        await context.StudentHomeworkTasks.AddAsync(studentHomeworkTask);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBasicDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var school = await SeedSchoolAsync(context);
        await SeedTestUsers(userManager, school);

        var subject = await SeedSubject(context, school);
        var schoolClass = await SeedClassAsync(context, subject);

        await SeedHomeworkTask(context, schoolClass);
    }

    private static async Task<Subject> SeedSubject(ApplicationDbContext context, School school)
    {
        var foundSubject = await context.Subjects.FindAsync(1);
        if (foundSubject is not null) return foundSubject;

        var subject = new Subject
        {
            Name = "Physics",
            School = school
        };

        await context.Subjects.AddAsync(subject);
        await context.SaveChangesAsync();
        return subject;
    }

    private static async Task<SchoolClass> SeedClassAsync(ApplicationDbContext context, Subject subject)
    {
        var foundClass = await context.Classes.FindAsync(1);
        if (foundClass is not null) return foundClass;

        var teacher = (await context.Users.FindAsync(TeacherUserId)) as Teacher;
        var student = (await context.Users.FindAsync(StudentUserId)) as Student;
        Debug.Assert(teacher != null);
        Debug.Assert(student != null);

        var schoolClass = new SchoolClass
        {
            Name = "13Y",
            Subject = subject,
            Teacher = teacher,
            Students = [student]
        };
        await context.Classes.AddAsync(schoolClass);
        await context.SaveChangesAsync();

        return schoolClass;
    }

    public static async Task InitialiseDataAsync(ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        await context.Database.EnsureCreatedAsync();

        await InitialiseRolesAsync(roleManager);
        await SeedBasicDataAsync(context, userManager);
    }
}