using WheresMyHomework.Core.Services.Homework.DTO;

namespace WheresMyHomework.Core.Services.TodoService;

public interface ITodoService
{
    Task<TodoResponseInfo> CreateNewTodoAsync(TodoRequestInfo todo);
    Task<bool> UpdateTodoStatusAsync(int todoId, bool newStatus);
    Task<bool> DeleteTodoAsync(int todoId);
    Task<bool> UpdateTodoDescriptionAsync(int todoId, string newDescription);
}