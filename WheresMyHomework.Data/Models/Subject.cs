using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WheresMyHomework.Data.Models;

public class Subject
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(20)] public required string Name { get; set; }

    [Required] public School School { get; set; } = null!;
    [Required, ForeignKey("SchoolId")] public int SchoolId { get; set; }
}