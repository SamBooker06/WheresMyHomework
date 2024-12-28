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
    Task<HomeworkResponseInfo> GetHomeworkInfoByIdAsync(int homeworkId);
    Task<StudentHomeworkResponseInfo> GetStudentHomeworkInfoByIdAsync(int homeworkId, string studentId);

    Task<ICollection<StudentHomeworkResponseInfo>> GetStudentHomeworkAsync(string studentId,
        StudentHomeworkFilter? filter=null);

    Task<ICollection<HomeworkResponseInfo>> GetHomeworkInfoByClassIdAsync(int classId);
    Task<bool> UpdateNotesAsync(int studentHomeworkId, string newNotes);
    Task<bool> UpdateHomeworkCompletionStatusAsync(int studentHomeworkId, bool isComplete);
    Task<bool> UpdateHomeworkPriorityAsync(int studentHomeworkId, Priority priority);
}

// TODO: Make UI
class CoffeeCup
{
    public bool IsEmpty { get; set; }

    public void Refill()
    {
        var message = IsEmpty ? "Refilling coffee cup" : "Not refilling coffee cup";
        Console.WriteLine(message);
    }

    public void Drink()
    {
        var message = IsEmpty ? "Drinking coffee" : "Not drinking coffee";
        Console.WriteLine(message);
    }
}