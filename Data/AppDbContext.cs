using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<TodoList>()
            .HasMany(l => l.Todos)
            .WithOne(t => t.List!)
            .HasForeignKey(t => t.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        // Map DueDate to "date" (no time) in Postgres
        b.Entity<Todo>()
            .Property(t => t.DueDate)
            .HasColumnType("date");
    }
}