using System.ComponentModel.DataAnnotations;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

// This represents a homework post that will be stored in the database
public class HomeworkTask
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(100), MinLength(5)]
    public required string Title { get; set; }

    [Required, MaxLength(500), MinLength(5)]
    public required string Description { get; set; }

    public DateTime SetDate { get; set; } = DateTime.Now;
    public required DateTime DueDate { get; set; } // TODO: Validate this is later than set date

    [Required] public SchoolClass Class { get; set; } = null!;
    public int ClassId { get; set; }

    public ICollection<StudentHomeworkTask> StudentHomeworkTasks { get; init; } = [];
}