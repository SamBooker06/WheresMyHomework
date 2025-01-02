using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public class StudentService(ApplicationDbContext context) : IStudentService
{
    public async Task<UserInfo> GetStudentInfoAsync(string studentId)
    {
        return await context.Users.OfType<Student>()
            .Where(student => student.Id == studentId)
            .Select(student => new UserInfo
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Id = student.Id,
                Title = student.Title,
                SchoolId = student.SchoolId,
            }).FirstAsync();
    }

    public async Task<ICollection<UserInfo>> GetStudentsBySchoolAsync(int schoolId)
    {
        return await context.Users.OfType<Student>()
            .Where(student => student.SchoolId == schoolId)
            .Select(student => new UserInfo
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Id = student.Id,
                Title = student.Title,
                SchoolId = student.SchoolId,
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<UserInfo>?> GetStudentsByClassAsync(int classId)
    {
        return await context.Classes.Where(cls => cls.Id == classId)
            .Select(cls => cls.Students.Select(student => new UserInfo
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Title = student.Title,
                SchoolId = student.SchoolId,
            })).FirstOrDefaultAsync();
    }
}