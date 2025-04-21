using Microsoft.EntityFrameworkCore;
using Todoapp.Entities;

namespace Todoapp.Context{

    public class ApplicationContext : DbContext {
        public ApplicationContext(DbContextOptions<ApplicationContext>options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}