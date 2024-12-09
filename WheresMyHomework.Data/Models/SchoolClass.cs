using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

public class SchoolClass
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(20), MinLength(1)]
    [Column(TypeName = "varchar(20)")] // Prevents unicode entry
    public required string Name { get; set; }

    [Required] public Subject Subject { get; set; } = null!;
    [ForeignKey("SubjectId"), MaxLength(36)]
    [Required] public int SubjectId { get; set; }
    [Required] public Teacher Teacher { get; set; } = null!;
    
    [ForeignKey("TeacherId"), MaxLength(36)]
    public string TeacherId { get; set; } = null!;

    public ICollection<Student> Students { get; init; } = [];
}