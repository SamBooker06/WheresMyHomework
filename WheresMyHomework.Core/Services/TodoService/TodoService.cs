using System.Diagnostics;
using WheresMyHomework.Core.Services.Homework.DTO;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.TodoService;

public class TodoService(ApplicationDbContext context) : ITodoService
{
    public async Task<TodoResponseInfo> CreateNewTodoAsync(TodoRequestInfo todo)
    {
        var task = await context.StudentHomeworkTasks.FindAsync(todo.StudentHomeworkTaskId); 
        Debug.Assert(task != null);

        var todoEntity = new Todo
        {
            IsComplete = todo.IsComplete,
            Description = todo.Description,
            StudentHomeworkTask = task
        };
        
        task.Todos.Add(todoEntity);

        await context.SaveChangesAsync();
        return new TodoResponseInfo
        {
            Description = todoEntity.Description,
            Id = todoEntity.Id,
            IsComplete = todoEntity.IsComplete
        };
    }

    public async Task<bool> UpdateTodoStatus(int todoId, bool newStatus)
    {
        // await context.StudentHomeworkTasks.
        return await Task.FromResult(true);
    }
}