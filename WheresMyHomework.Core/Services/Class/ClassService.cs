using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework;
using WheresMyHomework.Core.Services.Homework.DTO.Request;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Class;

public class ClassService(ApplicationDbContext context, IHomeworkService homeworkService) : IClassService
{
    public async Task<SchoolClassResponseInfo?> CreateClassAsync(CreateClassInfo info)
    {
        var students = info.StudentIds.Select(studentId =>
        {
            var student = new Student
            {
                Id = studentId,
            };
            
            // Attach tells the context that this entity already exists - do not make another one
            context.Attach(student);
            
            return student;
        });

        var createdClass = await context.Classes.AddAsync(new SchoolClass
        {
            Name = info.ClassName,
            TeacherId = info.TeacherId,
            SubjectId = info.SubjectId,
            Students = students.ToList(),
        });

        var changeCount = await context.SaveChangesAsync();
        
        // Return the new class if successful - otherwise, return null
        return changeCount > 0 ? new SchoolClassResponseInfo
        {
            Id = createdClass.Entity.Id,
            Name = createdClass.Entity.Name,
            TeacherId = createdClass.Entity.TeacherId,
            SubjectId = createdClass.Entity.SubjectId,
        } : null;
    }

    public async Task<IEnumerable<SchoolClassResponseInfo>> GetClassByTeacherAsync(string teacherId)
    {
        return await context.Classes.Where(c => c.TeacherId == teacherId)
            .Select(cls => new SchoolClassResponseInfo
            {
                Id = cls.Id,
                Name = cls.Name,
                SubjectId = cls.SubjectId,
                TeacherId = cls.TeacherId,
            }).ToListAsync();
    }

    public async Task<SchoolClassResponseInfo?> GetClassByIdAsync(int classId)
    {
        return await context.Classes.Where(c => c.Id == classId)
            .Select(cls => new SchoolClassResponseInfo
            {
                Id = cls.Id,
                Name = cls.Name,
                SubjectId = cls.SubjectId,
                TeacherId = cls.TeacherId,
            }).FirstOrDefaultAsync();
    }

    public async Task<bool> AddStudentToClass(int classId, string studentId)
    {
        var cls = await context.Classes.FindAsync(classId);
        if (cls == null) return false;

        var student = new Student
        {
            Id = studentId,
        };
        
        // This tells the context that this student already exists - don't make another one
        // This prevents us from having to find the actual student entity from the DB, reducing queries
        context.Attach(student);
        
        cls.Students.Add(student);

        // Update the student with all their missing homework tasks
        var homeworkTasks = await homeworkService.GetHomeworkByClassAsync(classId);
        foreach (var homeworkTask in homeworkTasks)
        {
            if (await homeworkService.GetStudentHomeworkInfoByIdAsync(homeworkTask.Id, studentId) is not null) continue;

            var taskInfo = new HomeworkRequestInfo
            {
                Description = homeworkTask.Description,
                Title = homeworkTask.Title,
                ClassId = classId,
                DueDate = homeworkTask.DueDate,
                SetDate = homeworkTask.SetDate,
            };
            //TODO Gives an error RAHHH.
            // await homeworkService.CreateStudentHomeworkTaskAsync(taskInfo, studentId);
        }

        return await context.SaveChangesAsync() > 0;
    }

    // Probably should add a flag to a homework task on whether the student is actually in the class, rather than deleting
    // from the database?? oh well
    public async Task<bool> RemoveStudentFromClass(int classId, string studentId)
    {
        // .Include() is the equivalent of INNER JOIN in SQl - needed when we need to get information from related entities
        var cls = await context.Classes.Include(cls => cls.Students)
            .FirstOrDefaultAsync(cls => cls.Id == classId);
        if (cls == null) return false;

        var student = await context.Users.OfType<Student>().FirstOrDefaultAsync(student => student.Id == studentId);
        if (student is null) return false;

        cls.Students.Remove(student);

        return await context.SaveChangesAsync() > 0;
    }
    
    
}
