using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public class TeacherService(ApplicationDbContext context) : ITeacherService
{
    public async Task<Teacher> GetTeacherByHomeworkIdAsync(int homeworkId)
    {
        return await context.HomeworkTasks.Where(task => task.Id == homeworkId)
            .Include(task => task.Class)
            .ThenInclude(cls => cls.Teacher)
            .Select(task => task.Class.Teacher).FirstAsync();
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
            SchoolId = teacher.SchoolId,
            Classes = teacher.Classes.Select(cls => new SchoolClassResponseInfo
            {
                Name = cls.Name,
                Id = cls.Id,
                Subject = new SubjectResponseInfo
                {
                    Name = cls.Subject.Name,
                },
                Teacher = new UserInfo
                {
                    Id = cls.TeacherId,
                    FirstName = cls.Teacher.FirstName,
                    LastName = cls.Teacher.LastName,
                    Title = cls.Teacher.Title,
                    SchoolId = cls.Teacher.SchoolId,
                }
            }).ToList()
        };
        return teacherInfo;
    }

    
}