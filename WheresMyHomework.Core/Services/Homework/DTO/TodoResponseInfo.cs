namespace WheresMyHomework.Core.Services.Homework.DTO;

public record TodoResponseInfo
{
    public required string Description { get; init; }
    public required bool IsComplete { get; set; }
    public required int Id {get ; init; }
}

