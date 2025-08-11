using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
public class TodosController(AppDbContext db) : ControllerBase
{
    [HttpGet("api/lists/{listId:guid}/todos")]
    public async Task<IActionResult> GetTodos(Guid listId)
    {
        var items = await db.Todos
            .Where(t => t.ListId == listId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
        return Ok(items);
    }

    public record CreateTodoDto(string Title, string? Description, DateTime? DueDate);

    [HttpPost("api/lists/{listId:guid}/todos")]
    public async Task<IActionResult> AddTodo(Guid listId, CreateTodoDto dto)
    {
        var exists = await db.TodoLists.AnyAsync(l => l.Id == listId);
        if (!exists) return NotFound();

        var todo = new Todo
        {
            ListId = listId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate?.Date // keep only date part
        };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return Ok(todo);
    }

    [HttpDelete("api/todos/{id:guid}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null) return NotFound();
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
