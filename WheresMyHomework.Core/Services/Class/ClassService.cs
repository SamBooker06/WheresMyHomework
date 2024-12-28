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
}