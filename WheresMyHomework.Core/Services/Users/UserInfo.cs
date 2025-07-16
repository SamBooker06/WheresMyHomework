using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Core.Services.Users;

public record UserInfo
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public required int SchoolId { get; init; }

    public required PersonTitle Title { get; init; }

    // This is used to generate the hash for use in hash tables
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    // Operator overloading to check whether to user entries are equal
    public virtual bool Equals(UserInfo? other) => Id == other?.Id;
}

