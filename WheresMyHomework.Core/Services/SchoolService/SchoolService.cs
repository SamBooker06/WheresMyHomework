using System.ComponentModel.DataAnnotations;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.SchoolService;

public class SchoolService(ApplicationDbContext context) : ISchoolService
{
    public async Task<int> CreateSchoolAsync(SchoolRequestInfo schoolInfo)
    {
        var school = await context.Schools.AddAsync(new School
        {
            Name = schoolInfo.Name
        });
        
        await context.SaveChangesAsync();
        return school.Entity.Id;
    }
}

public class SchoolRequestInfo
{
    [Required, MaxLength(36)] public string Name { get; set; } = string.Empty;
}

