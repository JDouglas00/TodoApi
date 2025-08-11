using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class TodoList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
