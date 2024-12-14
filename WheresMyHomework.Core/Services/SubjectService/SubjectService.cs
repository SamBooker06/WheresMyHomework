using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.SubjectService;

public class SubjectService(ApplicationDbContext context) : ISubjectService
{
    public async Task<SubjectResponseInfo> GetSubjectInfoAsync(int subjectId)
    {
        return await context.Subjects.Where(subject => subject.Id == subjectId)
            .Select(subject => new SubjectResponseInfo
            {
                Name = subject.Name,
                Id = subject.Id,
            }).FirstAsync();
    }

    public async Task<IEnumerable<SubjectResponseInfo>> GetSubjectsAsync(int schoolId)
    {
        return await context.Subjects.Where(subject => subject.SchoolId == schoolId).Select(subject =>
            new SubjectResponseInfo
            {
                Name = subject.Name,
                Id = subject.Id,
            }).ToListAsync();
    }

    public async Task<int> CreateSubjectAsync(SubjectCreateInfo info)
    {
        var subject = await context.Subjects.AddAsync(new Subject
        {
            Name = info.Name,
            SchoolId = info.SchoolId,
        });

        await context.SaveChangesAsync();
        return subject.Entity.Id;
    }
}

public record SubjectCreateInfo
{
    public string Name { get; set; }
    public int SchoolId { get; set; }
}