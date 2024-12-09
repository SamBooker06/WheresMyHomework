using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public class StudentService(ApplicationDbContext context) : IStudentService
{
    public async Task<Student?> GetStudentByIdAsync(string studentId)
    {
        return await context.FindAsync<Student>(studentId);
    }

    public async Task<ICollection<Student>> GetStudentsByTeacherAsync(Teacher teacher)
    {
        // Gets all students 
        // Where they have any classes with that teacher
        // And then removes duplicate results
        return await context.Users.OfType<Student>()
            .Where(student => student.SchoolClasses
                .Any(cls => cls.Teacher == teacher))
            .Distinct()
            .ToListAsync();
    }

    public async Task<ICollection<Student>> GetStudentsBySchoolAsync(School school)
    {
        return await context.Users.OfType<Student>()
            .Where(student => student.School == school)
            .ToListAsync();
    }
    
}