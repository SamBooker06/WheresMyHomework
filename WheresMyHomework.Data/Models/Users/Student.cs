namespace WheresMyHomework.Data.Models.Users;

public class Student : ApplicationUser
{
    public ICollection<SchoolClass> SchoolClasses { get; init; } = [];
}