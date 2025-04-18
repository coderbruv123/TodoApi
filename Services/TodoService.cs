using Microsoft.EntityFrameworkCore;
using Todoapp.Context;
using Todoapp.Entities;
using Todoapp.Services;

namespace Todoapp.Services{
    class TodoService(ApplicationContext context) : ITodoServices
    {
        
        public Task<Todo> AddTodoAsync(Todo todo)
        {
            context.Todos.Add(todo);
            context.SaveChangesAsync();
            return Task.FromResult(todo);
        }        
               

        public async Task<List<Todo>> GetTodosAsync()
        {
            var todos = await context.Todos.ToListAsync();
            return todos;
        }
        public async Task<Todo> UpdateTodoAsync(int id, Todo todo)
        {
            var existingTodo = await context.Todos.FindAsync(id);
            if (existingTodo is null)
            {
                return null;
            }
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.IsCompleted = todo.IsCompleted;
            existingTodo.UpdatedAt = DateTime.UtcNow;
            
             await context.SaveChangesAsync();
            return existingTodo;
        }
       
       
    }
}