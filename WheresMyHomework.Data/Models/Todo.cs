using System.ComponentModel.DataAnnotations;

namespace WheresMyHomework.Data.Models;

public class Todo
{
    [Key] public int Id { get; set; }

    public bool IsComplete { get; set; } = false;

    [Required, MaxLength(100), MinLength(3)]
    public required string Description { get; set; }
    
    [Required] public required StudentHomeworkTask StudentHomeworkTask { get; set; }
}