using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public class TeacherService(ApplicationDbContext context) : ITeacherService
{
    public async Task<TeacherInfo> GetTeacherByHomeworkIdAsync(int homeworkId)
    {
        return await context.HomeworkTasks.Where(task => task.Id == homeworkId)
            .Include(task => task.Class)
            .ThenInclude(cls => cls.Teacher)
            .Include(task => task.Class.Subject)
            .Select(task => new TeacherInfo
            {
                Id = task.Class.Teacher.Id,
                FirstName = task.Class.Teacher.FirstName,
                LastName = task.Class.Teacher.LastName,
                Title = task.Class.Teacher.Title,
                SchoolId = task.Class.Subject.SchoolId
            }).FirstAsync();
    }

    public async Task<TeacherInfo> GetTeacherInfoAsync(string teacherId)
    {
        var teacher = await context.Users.OfType<Teacher>().Include(teacher => teacher.Classes)
            .ThenInclude(cls => cls.Subject)
            .Where(teacher => teacher.Id == teacherId).FirstAsync();
        
        var teacherInfo = new TeacherInfo
        {
            Id = teacherId,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Title = teacher.Title,
            SchoolId = teacher.SchoolId
        };
        return teacherInfo;
    }

    public async Task<IEnumerable<TeacherInfo>> GetTeachersFromSchoolAsync(int schoolId)
    {
        return await context.Users.OfType<Teacher>().Where(teacher => teacher.SchoolId == schoolId)
            .Select(teacher => new TeacherInfo
            {
                Id = teacher.Id,
                Title = teacher.Title,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                SchoolId = teacher.SchoolId,
            }).ToListAsync();
    }
    
}