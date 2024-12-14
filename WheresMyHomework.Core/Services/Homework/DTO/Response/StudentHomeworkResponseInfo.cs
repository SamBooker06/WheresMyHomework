using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Homework.DTO.Response;

public record StudentHomeworkResponseInfo : HomeworkResponseInfo
{
    public required ICollection<TodoResponseInfo> Todos { get; init; }
    public required string Notes { get; init; }
    public required bool IsComplete { get; init; }
    public required Priority Priority { get; init; }
}