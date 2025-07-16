using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Request;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Homework;

public class HomeworkService(ApplicationDbContext context) : IHomeworkService
{
    public async Task CreateStudentHomeworkTaskAsync(HomeworkRequestInfo info, string studentId)
    {
        var task = new HomeworkTask
        {
            Title = info.Title,
            Description = info.Description,
            DueDate = info.DueDate,
            SetDate = info.SetDate,
            ClassId = info.ClassId,
        };
        // Wiss bombaclat dog i am
        var student = new Student
        {
            Id = studentId,
        };
        // context.Attach(student);

        await context.StudentHomeworkTasks.AddAsync(new StudentHomeworkTask
        {
            Student = student,
            HomeworkTask = task,
        });

        await context.SaveChangesAsync();
    }

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

    public async Task<HomeworkResponseInfo> GetHomeworkById(int homeworkId)
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

    public async Task<IEnumerable<HomeworkResponseInfo>> GetHomeworkByTeacherAsync(string teacherId)
    {
        return await context.HomeworkTasks
            .Include(task => task.Class)
            .Where(task => task.Class.TeacherId == teacherId)
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
                    SubjectId = task.Class.SubjectId,
                    TeacherId = teacherId
                }
            }).ToArrayAsync();
    }

    public async Task<StudentHomeworkResponseInfo?> GetStudentHomeworkInfoByIdAsync(int homeworkId, string studentId)
    {
        var homeworkInfo = await GetHomeworkById(homeworkId);
        var studentHomeworkTask = await context.StudentHomeworkTasks
            .Where(task => task.Student.Id == studentId && task.HomeworkTask.Id == homeworkId)
            .Include(studentHomeworkTask => studentHomeworkTask.Todos)
            .Include(studentHomeworkTask => studentHomeworkTask.Tags)
            .FirstOrDefaultAsync();

        return studentHomeworkTask is not null
            ? new StudentHomeworkResponseInfo
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
            }
            : null;
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

        // If there are priorities, modify query to only include tasks with matching priorities
        if (filter?.Priorities.Count > 0)
        {
            query = query.Where(task => filter.Priorities.Contains(task.Priority));
        }

        // Get all the entries in the DB matching this query
        var response = await query.ToArrayAsync();

        // Filter out any tasks without the matching tags, if there are any
        // This needs to be done after the query is run since the chosen backend (sqlite) does not support the statement
        // required to apply it directly to the query
        if (filter?.Tags.Count > 0)
        {
            response = response.Where(task => task.Tags.Any(tag => filter.Tags.Contains(tag.Name))).ToArray();
        }


        // Apply the title filter, if there is any
        // CurrentCultureIgnoreCase is used to ignore any capital letters when comparing the string and is built into
        // the standard library
        if (filter?.Title is not null && !string.IsNullOrWhiteSpace(filter.Title))
        {
            response = (filter.ExactMatch
                ? response.Where(task => task.Title.Equals(filter.Title, StringComparison.CurrentCultureIgnoreCase))
                : response.Where(task =>
                    task.Title.Contains(filter.Title, StringComparison.CurrentCultureIgnoreCase))).ToArray();
        }

        // After all filters are applied, return the tasks which were not filtered out
        return response;
    }

    public async Task<ICollection<HomeworkResponseInfo>> GetHomeworkByClassAsync(int classId)
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

    public async Task<bool> UpdateDescriptionAsync(int homeworkId, string newDescription)
    {
        var homeworkTask = await context.HomeworkTasks.FindAsync(homeworkId);
        if (homeworkTask is null) return false;

        homeworkTask.Description = newDescription;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTitleAsync(int homeworkId, string newTitle)
    {
        var homeworkTask = await context.HomeworkTasks.FindAsync(homeworkId);
        if (homeworkTask is null) return false;

        homeworkTask.Title = newTitle;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateDueDateAsync(int homeworkId, DateTime dueDate)
    {
        var homeworkTask = await context.HomeworkTasks.FindAsync(homeworkId);
        if (homeworkTask is null) return false;

        homeworkTask.DueDate = dueDate;
        return await context.SaveChangesAsync() > 0;
    }
}