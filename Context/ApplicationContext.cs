using Microsoft.EntityFrameworkCore;
using Todoapp.Entities;

namespace Todoapp.Context{

    public class ApplicationContext : DbContext {
        public ApplicationContext(DbContextOptions<ApplicationContext>options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .HasOne(t => t.User)
                .WithMany(u => u.Todos)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
   
    }
}