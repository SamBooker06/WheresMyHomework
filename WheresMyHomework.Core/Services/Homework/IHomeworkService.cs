using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Request;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Homework;

public interface IHomeworkService
{
    Task CreateHomeworkAsync(HomeworkRequestInfo info);
    Task DeleteHomeworkByIdAsync(int homeworkId);
    Task<HomeworkResponseInfo> GetHomeworkById(int homeworkId);
    Task<StudentHomeworkResponseInfo> GetStudentHomeworkInfoByIdAsync(int homeworkId, string studentId);

    Task<ICollection<StudentHomeworkResponseInfo>> GetStudentHomeworkAsync(string studentId,
        StudentHomeworkFilter? filter=null);

    Task<ICollection<HomeworkResponseInfo>> GetHomeworkByClassAsync(int classId);
    Task<bool> UpdateNotesAsync(int studentHomeworkId, string newNotes);
    Task<bool> UpdateHomeworkCompletionStatusAsync(int studentHomeworkId, bool isComplete);
    Task<bool> UpdateHomeworkPriorityAsync(int studentHomeworkId, Priority priority);
    Task<IEnumerable<HomeworkResponseInfo>> GetHomeworkByTeacherAsync(string teacherId);
    Task<bool> UpdateDescriptionAsync(int homeworkId, string newDescription);
    Task<bool> UpdateTitleAsync(int homeworkId, string newTitle);
    Task<bool> UpdateDueDateAsync(int homeworkId, DateTime dueDate);
}

