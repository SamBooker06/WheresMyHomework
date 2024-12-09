namespace WheresMyHomework.Core.Services.Auth;

public record AuthInfo
{
    public required string UserId { get; init; }
    
    public string? Email { get; init; }
}