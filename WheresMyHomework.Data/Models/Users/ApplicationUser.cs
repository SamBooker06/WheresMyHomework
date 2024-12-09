using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WheresMyHomework.Data.Models.Users;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(50), MinLength(3)]
    [Column(TypeName = "varchar(50)")]
    public required string FirstName { get; set; }

    [Required, MaxLength(50), MinLength(3)]
    [Column(TypeName = "varchar(50)")]
    public required string LastName { get; set; }

    [Required] public required School School { get; set; }
    public int SchoolId { get; set; }

    [Required] public required PersonTitle Title { get; set; }
}

public enum PersonTitle
{
    Mr,
    Miss,
    Mrs,
    Dr,
    Prof,
    Mx
}