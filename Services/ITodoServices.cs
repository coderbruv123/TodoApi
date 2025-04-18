using Todoapp.Entities;

namespace Todoapp.Services
{

    public interface ITodoServices
    {
        Task<Todo> AddTodoAsync(Todo todo);
        Task<List<Todo>> GetTodosAsync();
    }

}