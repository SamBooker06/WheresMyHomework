using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

public class Tag
{
    [StringLength(15)]
    public required string Name { get; set; }

    [ForeignKey("StudentId"), MaxLength(36)]
    public required string StudentId { get; set; }

    [Required] public Student Student { set; get; } = null!;

    public ICollection<StudentHomeworkTask> StudentHomeworkTasks { get; set; } = [];
}

