using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Class;

public class ClassService(ApplicationDbContext context) : IClassService
{
    public async Task<int> CreateClassAsync(CreateClassInfo info)
    {
        //TODO: Find why not working
        var createdClass = await context.Classes.AddAsync(new SchoolClass
        {
            Name = info.ClassName,
            TeacherId = info.TeacherId,
            SubjectId = info.SubjectId,
            // Students = info.StudentIds.Select(id => )
            
        });
        await context.SaveChangesAsync();
        return createdClass.Entity.Id;
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