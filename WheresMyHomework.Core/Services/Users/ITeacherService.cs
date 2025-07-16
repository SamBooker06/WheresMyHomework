namespace WheresMyHomework.Core.Services.Users;

public interface ITeacherService
{
    Task<TeacherInfo> GetTeacherByHomeworkIdAsync(int homeworkId);
    Task<TeacherInfo> GetTeacherInfoAsync(string teacherId);
    Task<IEnumerable<TeacherInfo>> GetTeachersFromSchoolAsync(int schoolId);
}
