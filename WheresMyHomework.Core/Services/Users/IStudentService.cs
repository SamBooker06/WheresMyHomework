using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

// TODO: Move to DTOs
public interface IStudentService
{
    Task<Student?> GetStudentByIdAsync(string studentId);
    
    Task<ICollection<Student>> GetStudentsByTeacherAsync(Teacher teacher);
    Task<ICollection<Student>> GetStudentsBySchoolAsync(School school);

}