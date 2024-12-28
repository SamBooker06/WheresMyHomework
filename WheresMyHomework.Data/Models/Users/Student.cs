namespace WheresMyHomework.Data.Models.Users;

public class Student : ApplicationUser
{
    public ICollection<SchoolClass> SchoolClasses { get; init; } = [];
    public ICollection<Tag> TaskTags { get; init; } = [];
}