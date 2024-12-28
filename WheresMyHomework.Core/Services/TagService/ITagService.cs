using WheresMyHomework.Core.Services.Homework.DTO.Response;

namespace WheresMyHomework.Core.Services.TagService;

public interface ITagService
{
    Task<TagResponseInfo?> AddTagAsync(TagRequestInfo tagInfo);
    Task<IEnumerable<TagResponseInfo>> GetTagsAsync(string studentId);
    Task<bool> DeleteTagAsync(int studentHomeworkId, string tagName);
}