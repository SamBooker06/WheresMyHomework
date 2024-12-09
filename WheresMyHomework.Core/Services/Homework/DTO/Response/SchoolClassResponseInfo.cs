using WheresMyHomework.Core.Services.Users;

namespace WheresMyHomework.Core.Services.Homework.DTO.Response;

public record SchoolClassResponseInfo
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required SubjectResponseInfo Subject { get; init; }
    public required UserInfo Teacher { get; init; }
}