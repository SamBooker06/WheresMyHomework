namespace WheresMyHomework.Core.Services.SubjectService;

public record SubjectResponseInfo
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}