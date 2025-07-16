using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.TagService;

public class TagService(ApplicationDbContext context) : ITagService
{
    public async Task<TagResponseInfo?> AddTagAsync(TagRequestInfo tagInfo)
    {
        var task = await context.StudentHomeworkTasks.FindAsync(tagInfo.StudentHomeworkId);
        if (task is null) return null;

        var existingTag = await context.Tags.FindAsync(tagInfo.Name, task.StudentId);
        if (existingTag is not null)
        {
            existingTag.StudentHomeworkTasks.Add(task);
        }
        else
        {
            task.Tags.Add(new Tag
            {
                Name = tagInfo.Name,
                StudentId = task.StudentId,
            });
        }

        await context.SaveChangesAsync();

        return new TagResponseInfo
        {
            StudentId = task.StudentId,
            Name = tagInfo.Name,
        };
    }

    public async Task<IEnumerable<TagResponseInfo>> GetTagsAsync(string studentId)
    {
        return await context.Tags.Where(tag => tag.StudentId == studentId)
            .Select(tag => new TagResponseInfo
            {
                StudentId = tag.StudentId,
                Name = tag.Name,
            }).ToListAsync();
    }

    public async Task<bool> DeleteTagAsync(int studentHomeworkId, string tagName)
    {
        var task = await context.StudentHomeworkTasks.Include(studentHomeworkTask => studentHomeworkTask.Tags)
            .FirstOrDefaultAsync(task => task.Id == studentHomeworkId);
        if (task is null)
        {
            return false;
        }

        var tagEntry = await context.Tags.Include(tag => tag.StudentHomeworkTasks)
            .Where(tag => tag.StudentId == task.StudentId && tag.Name == tagName)
            .FirstOrDefaultAsync();
        if (tagEntry is null) return false;

        // Remove the tag from the task
        task.Tags.Remove(task.Tags.Single(tag => tag.Name == tagName));

        // If there are no tasks with this tag remaining, delete the tag entirely
        if (tagEntry.StudentHomeworkTasks.Count == 0) context.Tags.Remove(tagEntry);

        return await context.SaveChangesAsync() > 0;
    }
}