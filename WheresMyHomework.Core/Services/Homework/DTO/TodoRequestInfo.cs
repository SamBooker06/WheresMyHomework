namespace WheresMyHomework.Core.Services.Homework.DTO;

public record TodoRequestInfo
{
    public string Description { get; set; } = string.Empty;
    public int StudentHomeworkTaskId { get; set; }
    public bool IsComplete { get; set; }
}