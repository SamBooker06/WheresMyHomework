using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Class;

public interface IClassService
{
    public Task<int> CreateClassAsync(CreateClassInfo info);
}