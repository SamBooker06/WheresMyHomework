using WheresMyHomework.Core.Services.Homework.DTO.Response;

namespace WheresMyHomework.Core.Services.SubjectService;

public interface ISubjectService
{
    Task<SubjectResponseInfo> GetSubjectInfoAsync(int subjectId);
    Task<IEnumerable<SubjectResponseInfo>> GetSubjectsAsync(int schoolId);
    Task<int> CreateSubjectAsync(SubjectCreateInfo info);
}