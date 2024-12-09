using System.ComponentModel.DataAnnotations;

namespace WheresMyHomework.Data.Models;

public class School
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(100), MinLength(3)]
    public required string Name { get; set; }
}