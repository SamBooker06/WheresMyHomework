using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
internal class LessThanDateAttribute(string comparisonPropertyName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        Debug.Assert(value != null);

        ErrorMessage = ErrorMessageString;
        var currentValue = (DateTime)value;

        var property = validationContext.ObjectType.GetProperty(comparisonPropertyName);
        Debug.Assert(property != null);
        var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

        return comparisonValue < currentValue ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
    }
}

// This represents a homework post that will be stored in the database
public class HomeworkTask
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(100), MinLength(5)]
    public required string Title { get; set; }

    [Required, MaxLength(500), MinLength(5)]
    public required string Description { get; set; }

    public DateTime SetDate { get; set; } = DateTime.Now;

    [LessThanDate("SetDate")]
    public required DateTime DueDate { get; set; }

    [Required] public SchoolClass Class { get; set; } = null!;
    public int ClassId { get; set; }

    public ICollection<StudentHomeworkTask> StudentHomeworkTasks { get; init; } = [];
}