using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WheresMyHomework.Data.Models.Users;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(50), MinLength(3)]
    [Column(TypeName = "varchar(50)")]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50), MinLength(3)]
    [Column(TypeName = "varchar(50)")]
    public string LastName { get; set; } = string.Empty;

    [Required] public School? School { get; set; }
    public int SchoolId { get; set; }

    [Required] public PersonTitle Title { get; set; }
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