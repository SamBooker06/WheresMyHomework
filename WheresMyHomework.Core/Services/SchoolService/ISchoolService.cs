namespace WheresMyHomework.Core.Services.SchoolService;

public interface ISchoolService
{
    Task<int> CreateSchoolAsync(SchoolRequestInfo schoolInfo);
}
