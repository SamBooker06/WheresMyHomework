using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Class;

public interface IClassService
{
    public Task<int> CreateClassAsync(CreateClassInfo info);
    public Task<IEnumerable<SchoolClassResponseInfo>> GetClassByTeacherAsync(string teacherId);

    public Task<SchoolClassResponseInfo?> GetClassByIdAsync(int classId);
    Task<bool> AddStudentToClass(int classId, string studentId);
    Task<bool> RemoveStudentFromClass(int classId, string studentId);
}