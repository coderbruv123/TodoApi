using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Todoapp.Context;
using Todoapp.Entities;
using Todoapp.Services;

namespace Todoapp.Services{
    class TodoService(ApplicationContext context) : ITodoServices
    {
        
        public async Task<Todo?> AddTodoAsync(Todo todo)
        {
          context.Todos.Add(todo);
               await context.SaveChangesAsync();
            return todo;
        }        
               

        public async Task<List<Todo>> GetTodosbyIdAsync(Guid userid)
        {
            var todos = await context.Todos.Where(t => t.UserId == userid).ToListAsync();
            return todos;
        }
     

        public async Task<Todo?> GetTodoAsync(Guid id)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(t=>t.Id == id);
            if(todo is null)
            {
                return null;
            }
            return todo;
        }
        public async Task<Todo?> DeleteTodoAsync(Guid id, Guid userId)
        {
            var todo = await context.Todos.FirstOrDefaultAsync(t=>t.Id == id);
            if(todo is null)
            {
                return null;
            }
            if(todo.UserId != userId)
            {
                return null;
            }
            context.Todos.Remove(todo);
            return todo;
        }
        public async Task<Todo?> UpdateTodoAsync(Guid id, Todo todo)
        {
            if(id != todo.Id)
            {
                return null;
            }

            
            var existingTodo = await context.Todos.FirstOrDefaultAsync(t=>t.Id == id);
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