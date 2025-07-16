namespace WheresMyHomework.Core.Services.SubjectService;

public record SubjectCreateInfo
{
    public string Name { get; set; }
    public int SchoolId { get; set; }
}
