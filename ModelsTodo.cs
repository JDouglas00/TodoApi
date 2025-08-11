namespace TodoApi.Models;

public class Todo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public bool Completed { get; set; } = false;

    // Store as "date" in Postgres; we'll map it below
    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public TodoList? List { get; set; }
}
