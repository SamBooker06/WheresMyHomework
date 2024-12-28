using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

// This represents a HomeworkTask from the perspective of a student
// This means it will have student-specific info, such as priority and notes
public class StudentHomeworkTask
{
    [Key] public int Id { get; set; }

    [Required] public required Student Student { get; set; }

    [ForeignKey("StudentId"), MaxLength(36)]
    public string StudentId { get; set; } = string.Empty;

    [Required] public required HomeworkTask HomeworkTask { get; set; }

    [Required, MaxLength(300)] public string Notes { get; set; } = string.Empty;

    [Required] public Priority Priority { get; set; } = Priority.None;
    [Required] public ICollection<Todo> Todos { get; set; } = [];
    [Required] public bool IsComplete { get; set; }
    [Required] public ICollection<Tag> Tags { get; set; } = [];
}

public enum Priority
{
    None,
    Low,
    Medium,
    High
}