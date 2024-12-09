using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public interface ITeacherService
{
    Task<Teacher> GetTeacherByHomeworkIdAsync(int homeworkId);
    Task<TeacherInfo> GetTeacherInfoAsync(string teacherId);
}