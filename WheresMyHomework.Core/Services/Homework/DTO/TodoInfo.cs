namespace WheresMyHomework.Core.Services.Homework.DTO;

public record TodoInfo
{
    public required string Description { get; init; }
    public required bool IsComplete { get; set; }
}