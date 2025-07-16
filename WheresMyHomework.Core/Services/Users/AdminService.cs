using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public class AdminService(ApplicationDbContext context) : IAdminService
{
    public async Task<AdminInfo> GetAdminInfoAsync(string adminId)
    {
        return await context.Users.OfType<Admin>().Where(user => user.Id == adminId)
            .Select(admin => new AdminInfo
            {
                Id = admin.Id,
                Title = admin.Title,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                SchoolId = admin.SchoolId,
            }).FirstAsync();
    }
}

public record AdminInfo : UserInfo
{
    // This is empty but allows for adding additional data specific to admin users
}
