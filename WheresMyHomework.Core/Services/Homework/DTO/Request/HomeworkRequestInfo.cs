namespace WheresMyHomework.Core.Services.Homework.DTO.Request;

public record HomeworkRequestInfo
{
    public required string Title { get; set;  }
    
    public required DateTime DueDate { get; set; }
    public required DateTime SetDate { get; init; } = DateTime.Now;
    
    public required string Description { get; set; }
    public required int ClassId { get; init; }
}
