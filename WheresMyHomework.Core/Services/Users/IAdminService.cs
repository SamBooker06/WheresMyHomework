namespace WheresMyHomework.Core.Services.Users;

public interface IAdminService
{
    Task<AdminInfo> GetAdminInfoAsync(string adminId);
}

