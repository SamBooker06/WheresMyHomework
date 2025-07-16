using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public interface IStudentService
{
    Task<UserInfo?> GetStudentInfoAsync(string studentId);
    
    Task<ICollection<UserInfo>> GetStudentsBySchoolAsync(int schoolId);

    Task<IEnumerable<UserInfo>?> GetStudentsByClassAsync(int classId);
}
