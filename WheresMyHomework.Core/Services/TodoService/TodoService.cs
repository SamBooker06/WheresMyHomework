using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> UpdateTodoStatusAsync(int todoId, bool newStatus)
    {
        var todo = await context.Todos.FindAsync(todoId);
        if (todo is null) return false;
        
        todo.IsComplete = newStatus;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTodoAsync(int todoId)
    {
        var todo = await context.Todos.FindAsync(todoId);
        if (todo is null) return false;
        
        context.Todos.Remove(todo);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTodoDescriptionAsync(int todoId, string newDescription)
    {
        var todo = await context.Todos.FindAsync(todoId);
        if (todo is null) return false;
        
        todo.Description = newDescription;
        
        return await context.SaveChangesAsync() > 0;
    }
}