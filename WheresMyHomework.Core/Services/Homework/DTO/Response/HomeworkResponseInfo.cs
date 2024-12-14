namespace WheresMyHomework.Core.Services.Homework.DTO.Response;

public record HomeworkResponseInfo
{
    public required string Title { get; init;  }
    public required int Id { get; init; }
    
    public required DateTime DueDate { get; init; }
    public required DateTime SetDate { get; init; }
    
    public required string Description { get; init; }
    public required SchoolClassResponseInfo Class { get; init; }
}