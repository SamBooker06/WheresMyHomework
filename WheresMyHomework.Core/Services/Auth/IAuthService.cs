namespace WheresMyHomework.Core.Services.Auth;

public interface IAuthService
{
    Task<AuthInfo?> GetAuthenticatedUserInfoAsync();
    Task<bool> IsAuthenticatedAsync();
}