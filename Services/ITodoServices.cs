using Todoapp.Entities;

namespace Todoapp.Services
{

    public interface ITodoServices
    {
        Task<Todo?> AddTodoAsync(Todo todo);
        Task<List<Todo>> GetTodosAsync();
        Task<Todo?> GetTodoAsync(int id);
        Task<Todo?> DeleteTodoAsync(int id);
        Task<Todo?> UpdateTodoAsync(int id, Todo todo);
    }

}