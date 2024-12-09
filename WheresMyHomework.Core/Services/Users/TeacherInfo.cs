using WheresMyHomework.Core.Services.Homework.DTO.Response;

namespace WheresMyHomework.Core.Services.Users;

public record TeacherInfo : UserInfo
{
    public required ICollection<SchoolClassResponseInfo> Classes { get; init; }
}