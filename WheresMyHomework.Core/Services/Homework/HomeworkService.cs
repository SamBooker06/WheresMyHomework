using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Core.Services.Homework.DTO.Request;
using WheresMyHomework.Core.Services.Homework.DTO.Response;
using WheresMyHomework.Core.Services.Users;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.Homework;

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

    // TODO: Move to an update info record
    public async Task UpdateHomeworkAsync(HomeworkTask homeworkTask)
    {
        context.HomeworkTasks.Update(homeworkTask);
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
            ClassResponse = new SchoolClassResponseInfo
            {
                Name = homeworkTask.Class.Name,
                Id = homeworkTask.Class.Id,
                Teacher = new UserInfo
                {
                    FirstName = homeworkTask.Class.Teacher.FirstName,
                    LastName = homeworkTask.Class.Teacher.LastName,
                    Title = homeworkTask.Class.Teacher.Title,
                    Id = homeworkTask.Class.Teacher.Id,
                    SchoolId = homeworkTask.Class.Subject.SchoolId
                },
                Subject = new SubjectResponseInfo
                {
                    Name = homeworkTask.Class.Subject.Name,
                },
            }
        };
    }

    public async Task<StudentHomeworkResponseInfo> GetStudentHomeworkInfoByIdAsync(int homeworkId, string studentId)
    {
        var homeworkInfo = await GetHomeworkInfoByIdAsync(homeworkId);
        var studentHomeworkTask = await context.StudentHomeworkTasks
            .Where(task => task.Student.Id == studentId && task.HomeworkTask.Id == homeworkId)
            .Include(studentHomeworkTask => studentHomeworkTask.Todos)
            .FirstAsync();

        return new StudentHomeworkResponseInfo
        {
            Title = homeworkInfo.Title,
            Id = homeworkInfo.Id,
            ClassResponse = homeworkInfo.ClassResponse,
            Notes = studentHomeworkTask.Notes,
            IsComplete = studentHomeworkTask.IsComplete,
            Description = homeworkInfo.Description,
            DueDate = homeworkInfo.DueDate,
            SetDate = homeworkInfo.SetDate,
            Todos = studentHomeworkTask.Todos
                .Select(todo => new TodoInfo
                {
                    Description = todo.Description,
                    IsComplete = todo.IsComplete
                }).ToList()
        };
    }

    public async Task<ICollection<StudentHomeworkResponseInfo>> GetStudentHomeworkAsync(string studentId,
        StudentHomeworkFilter? filter = null)
    {
        return await context.StudentHomeworkTasks
            .Where(task => task.Student.Id == studentId)
            .Where(task => filter == null || // If there is a filter
                           filter.Title == null ||
                           filter.Title == string.Empty || // and the title is not empty
                           ((filter.ExactMatch && // If exact match
                             task.HomeworkTask.Title == filter.Title) || // else
                            task.HomeworkTask.Title.Contains(filter.Title)) &&
                           filter.Priorities.Contains(task.Priority)) // Check if is of correct priority
            .Include(st => st.HomeworkTask)
            .ThenInclude(task => task.Class)
            .ThenInclude(task => task.Teacher)
            .Include(st => st.HomeworkTask.Class.Subject)
            .Include(task => task.Todos)
            .Select(st => new StudentHomeworkResponseInfo
            {
                Title = st.HomeworkTask.Title,
                Id = st.HomeworkTask.Id,
                Notes = st.Notes,
                Description = st.HomeworkTask.Description,
                DueDate = st.HomeworkTask.DueDate,
                SetDate = st.HomeworkTask.SetDate,
                ClassResponse = new SchoolClassResponseInfo
                {
                    Name = st.HomeworkTask.Class.Name,
                    Id = st.HomeworkTask.Class.Id,
                    Teacher = new UserInfo
                    {
                        FirstName = st.HomeworkTask.Class.Teacher.FirstName,
                        LastName = st.HomeworkTask.Class.Teacher.LastName,
                        Title = st.HomeworkTask.Class.Teacher.Title,
                        Id = st.HomeworkTask.Class.Teacher.Id,
                        SchoolId = st.HomeworkTask.Class.Subject.SchoolId
                    },
                    Subject = new SubjectResponseInfo
                    {
                        Name = st.HomeworkTask.Class.Subject.Name,
                    }
                },
                IsComplete = st.IsComplete,
                Todos = st.Todos.Select(todo => new TodoInfo
                {
                    Description = todo.Description,
                    IsComplete = todo.IsComplete
                }).ToList()
            }).ToArrayAsync();
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
                ClassResponse = new SchoolClassResponseInfo
                {
                    Name = task.Class.Name,
                    Id = task.Class.Id,
                    Subject = new SubjectResponseInfo
                    {
                        Name = task.Class.Subject.Name,
                    },
                    Teacher = new UserInfo
                    {
                        FirstName = task.Class.Teacher.FirstName,
                        LastName = task.Class.Teacher.LastName,
                        Title = task.Class.Teacher.Title,
                        Id = task.Class.Teacher.Id,
                        SchoolId = task.Class.Subject.SchoolId
                    }
                }
            }).ToArrayAsync();
    }
}