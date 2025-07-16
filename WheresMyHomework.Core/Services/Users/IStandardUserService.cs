namespace WheresMyHomework.Core.Services.Users;

public interface IStandardUserService
{
    Task<UserInfo?> GetUserInfoAsync(string userId);
}
