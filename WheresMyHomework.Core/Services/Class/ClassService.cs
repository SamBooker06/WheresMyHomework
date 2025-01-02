using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Class;

public class ClassService(ApplicationDbContext context) : IClassService
{
    public async Task<int> CreateClassAsync(CreateClassInfo info)
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

        // -1 denotes that no class was made
        return changeCount > 0 ? createdClass.Entity.Id : -1;
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
        context.Attach(student);
        cls.Students.Add(student);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveStudentFromClass(int classId, string studentId)
    {
        var cls = await context.Classes.Include(cls => cls.Students).FirstOrDefaultAsync(cls => cls.Id == classId);
        if (cls == null) return false;

        var student = await context.Users.OfType<Student>().FirstOrDefaultAsync(student => student.Id == studentId);
        if (student is null) return false;

        cls.Students.Remove(student);

        return await context.SaveChangesAsync() > 0;
    }
}