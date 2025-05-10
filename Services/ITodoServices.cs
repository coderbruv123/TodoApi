using Todoapp.Entities;

namespace Todoapp.Services
{

    public interface ITodoServices
    {
        Task<Todo?> AddTodoAsync(Todo todo);
        Task<List<Todo>> GetTodosbyIdAsync(Guid userid);
        Task<Todo?> GetTodoAsync(Guid id);
        Task<Todo?> DeleteTodoAsync(Guid id,Guid userId);
        Task<Todo?> UpdateTodoAsync(Guid id, Todo todo);
    }

}