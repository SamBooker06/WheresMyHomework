using WheresMyHomework.Core.Services.Homework.DTO;

namespace WheresMyHomework.Core.Services.TodoService;

public interface ITodoService
{
    Task<TodoResponseInfo> CreateNewTodoAsync(TodoRequestInfo todo);
}