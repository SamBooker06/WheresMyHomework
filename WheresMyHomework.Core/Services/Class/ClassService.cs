using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Class;

public class ClassService(ApplicationDbContext context) : IClassService
{
    public async Task<int> CreateClassAsync(CreateClassInfo info)
    {
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
}