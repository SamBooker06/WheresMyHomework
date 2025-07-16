using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;

namespace WheresMyHomework.Core.Services.Users;

public class StandardUserService(ApplicationDbContext context) : IStandardUserService
{
    public async Task<UserInfo?> GetUserInfoAsync(string userId)
    {
        return await context.Users.Where(user => user.Id == userId)
            .Select(user => new UserInfo
            {
                Id = user.Id,
                Title = user.Title,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SchoolId = user.SchoolId,
            }).FirstOrDefaultAsync();
    }
}
