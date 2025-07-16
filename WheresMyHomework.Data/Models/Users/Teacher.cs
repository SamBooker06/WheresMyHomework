using System.ComponentModel.DataAnnotations;

namespace WheresMyHomework.Data.Models.Users;

public class Teacher : ApplicationUser
{
    [Required] public ICollection<SchoolClass> Classes { get; set; } = [];
}
