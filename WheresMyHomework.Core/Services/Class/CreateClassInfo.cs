namespace WheresMyHomework.Core.Services.Class;

public record CreateClassInfo
{
    public required string ClassName { get; set; }

    public required string TeacherId { get; set; }
    
    public required int SubjectId { get; set; }
    
    public ICollection<int> StudentIds { get; set; } = new List<int>();
}