using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/lists")]
public class TodoListsController(AppDbContext db) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetLists()
	{
		var lists = await db.TodoLists
			.OrderByDescending(l => l.CreatedAt)
			.Select(l => new {
				id = l.Id,
				name = l.Name,
				createdAt = l.CreatedAt,
				updatedAt = l.UpdatedAt,
				todos = l.Todos.Select(t => new {
					id = t.Id,
					title = t.Title,
					description = t.Description,
					dueDate = t.DueDate,
					completed = t.Completed,
					createdAt = t.CreatedAt,
					updatedAt = t.UpdatedAt
				}).ToList()
			})
			.ToListAsync();

		return Ok(lists);
	}


    public record CreateListDto(string Name);

    [HttpPost]
    public async Task<IActionResult> CreateList(CreateListDto dto)
    {
        var list = new TodoList { Name = dto.Name };
        db.TodoLists.Add(list);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLists), new { id = list.Id }, list);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteList(Guid id)
    {
        var list = await db.TodoLists.FindAsync(id);
        if (list is null) return NotFound();
        db.TodoLists.Remove(list);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
