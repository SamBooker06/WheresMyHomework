using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Request;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Homework;

// TODO: Move to StudentHomeworkService
public class HomeworkService(ApplicationDbContext context) : IHomeworkService
{
    public async Task CreateHomeworkAsync(HomeworkRequestInfo info)
    {
        var task = new HomeworkTask
        {
            Title = info.Title,
            Description = info.Description,
            DueDate = info.DueDate,
            SetDate = info.SetDate,
            ClassId = info.ClassId,
        };
        await context.HomeworkTasks.AddAsync(task);

        // TODO: Find if anyway to do this with OnModelCreating
        // Get all the students in the class
        var students = await context.Classes
            .Include(cls => cls.Students)
            .Where(cls => cls.Id == info.ClassId)
            .Select(cls => cls.Students).FirstAsync();

        // Create a student homework task for each homework task
        foreach (var student in students)
        {
            await context.StudentHomeworkTasks.AddAsync(new StudentHomeworkTask
            {
                Student = student,
                HomeworkTask = task,
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteHomeworkByIdAsync(int homeworkId)
    {
        await context.HomeworkTasks.Where(task => task.Id == homeworkId).ExecuteDeleteAsync();
        await context.SaveChangesAsync();
    }

    public async Task<HomeworkResponseInfo> GetHomeworkInfoByIdAsync(int homeworkId)
    {
        var homeworkTask = await context.HomeworkTasks.Include(task => task.Class)
            .ThenInclude(cls => cls.Teacher).Include(homeworkTask => homeworkTask.Class)
            .ThenInclude(schoolClass => schoolClass.Subject)
            .Where(task => task.Id == homeworkId)
            .FirstAsync();

        return new HomeworkResponseInfo
        {
            Title = homeworkTask.Title,
            Id = homeworkTask.Id,
            Description = homeworkTask.Description,
            DueDate = homeworkTask.DueDate,
            SetDate = homeworkTask.SetDate,
            Class = new SchoolClassResponseInfo
            {
                Name = homeworkTask.Class.Name,
                Id = homeworkTask.Class.Id,
                TeacherId = homeworkTask.Class.Teacher.Id,
                SubjectId = homeworkTask.Class.Subject.Id,
            }
        };
    }

    public async Task<StudentHomeworkResponseInfo> GetStudentHomeworkInfoByIdAsync(int homeworkId, string studentId)
    {
        var homeworkInfo = await GetHomeworkInfoByIdAsync(homeworkId);
        var studentHomeworkTask = await context.StudentHomeworkTasks
            .Where(task => task.Student.Id == studentId && task.HomeworkTask.Id == homeworkId)
            .Include(studentHomeworkTask => studentHomeworkTask.Todos)
            .Include(studentHomeworkTask => studentHomeworkTask.Tags)
            .FirstAsync();

        return new StudentHomeworkResponseInfo
        {
            Title = homeworkInfo.Title,
            Id = homeworkInfo.Id,
            StudentHomeworkId = studentHomeworkTask.Id,
            Class = homeworkInfo.Class,
            Notes = studentHomeworkTask.Notes,
            IsComplete = studentHomeworkTask.IsComplete,
            Description = homeworkInfo.Description,
            DueDate = homeworkInfo.DueDate,
            SetDate = homeworkInfo.SetDate,
            Priority = studentHomeworkTask.Priority,
            Tags = studentHomeworkTask.Tags.Select(tag => new TagResponseInfo
            {
                Name = tag.Name,
                StudentId = studentHomeworkTask.StudentId,
            }).ToList(),
            Todos = studentHomeworkTask.Todos
                .Select(todo => new TodoResponseInfo
                {
                    Description = todo.Description,
                    IsComplete = todo.IsComplete,
                    Id = todo.Id
                }).ToList()
        };
    }

    public async Task<ICollection<StudentHomeworkResponseInfo>> GetStudentHomeworkAsync(string studentId,
        StudentHomeworkFilter? filter = null)
    {
        var query = context.StudentHomeworkTasks
            .Where(task => task.Student.Id == studentId)
            .Include(st => st.HomeworkTask)
            .ThenInclude(task => task.Class)
            .ThenInclude(task => task.Teacher)
            .Include(st => st.HomeworkTask.Class.Subject)
            .Include(task => task.Todos)
            .Include(st => st.Tags)
            .Select(st => new StudentHomeworkResponseInfo
            {
                Title = st.HomeworkTask.Title,
                Id = st.HomeworkTask.Id,
                StudentHomeworkId = st.HomeworkTask.Id,
                Notes = st.Notes,
                Description = st.HomeworkTask.Description,
                DueDate = st.HomeworkTask.DueDate,
                SetDate = st.HomeworkTask.SetDate,
                Priority = st.Priority,
                Tags = st.Tags.Select(tag => new TagResponseInfo
                {
                    Name = tag.Name,
                    StudentId = studentId,
                }).ToList(),
                Class = new SchoolClassResponseInfo
                {
                    Name = st.HomeworkTask.Class.Name,
                    Id = st.HomeworkTask.Class.Id,
                    TeacherId = st.HomeworkTask.Class.TeacherId,
                    SubjectId = st.HomeworkTask.Class.SubjectId,
                },
                IsComplete = st.IsComplete,
                Todos = st.Todos.Select(todo => new TodoResponseInfo
                {
                    Description = todo.Description,
                    IsComplete = todo.IsComplete,
                    Id = todo.Id
                }).ToList()
            });
        
        if (filter?.Priorities.Count > 0)
        {
            query = query.Where(task => filter.Priorities.Contains(task.Priority));
        }


        var response = await query.ToArrayAsync();
        
        if (filter?.Tags.Count > 0)
        {
            response = response.Where(task => task.Tags.Any(tag => filter.Tags.Contains(tag.Name))).ToArray();
        }


        if (filter?.Title is not null && !string.IsNullOrWhiteSpace(filter.Title))
        {
            response = (filter.ExactMatch
                    ? response.Where(task => task.Title.Equals(filter.Title, StringComparison.CurrentCultureIgnoreCase))
                    : response.Where(task =>
                        task.Title.Contains(filter.Title, StringComparison.CurrentCultureIgnoreCase))).ToArray();
        }

        return response;
    }

    public async Task<ICollection<HomeworkResponseInfo>> GetHomeworkInfoByClassIdAsync(int classId)
    {
        return await context.HomeworkTasks.Include(task => task.Class)
            .ThenInclude(cls => cls.Teacher)
            .Include(homeworkTask => homeworkTask.Class)
            .ThenInclude(schoolClass => schoolClass.Subject)
            .Where(task => task.ClassId == classId)
            .Select(task => new HomeworkResponseInfo
            {
                Title = task.Title,
                Id = task.Id,
                Description = task.Description,
                DueDate = task.DueDate,
                SetDate = task.SetDate,
                Class = new SchoolClassResponseInfo
                {
                    Name = task.Class.Name,
                    Id = task.Class.Id,
                    SubjectId = task.Class.Subject.Id,
                    TeacherId = task.Class.Teacher.Id,
                }
            }).ToArrayAsync();
    }

    public async Task<bool> UpdateNotesAsync(int studentHomeworkId, string newNotes)
    {
        var task = await context.StudentHomeworkTasks.FindAsync(studentHomeworkId);
        if (task is null) return false;

        task.Notes = newNotes;

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateHomeworkCompletionStatusAsync(int studentHomeworkId, bool isComplete)
    {
        var task = await context.StudentHomeworkTasks.FindAsync(studentHomeworkId);
        if (task is null) return false;

        task.IsComplete = isComplete;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateHomeworkPriorityAsync(int studentHomeworkId, Priority priority)
    {
        var homeworkTask = await context.StudentHomeworkTasks.FindAsync(studentHomeworkId);
        if (homeworkTask is null) return false;

        homeworkTask.Priority = priority;
        return await context.SaveChangesAsync() > 0;
    }
}

// Where(task => filter == null || // If there is a filter
//               filter.Title == null ||
//               filter.Title == string.Empty || // and the title is not empty
//               ((filter.ExactMatch && // If exact match
//                 task.HomeworkTask.Title == filter.Title) || // else
//                task.HomeworkTask.Title.Contains(filter.Title)) &&
//               filter.Priorities.Contains(task.Priority)) // Check if is of correct priority