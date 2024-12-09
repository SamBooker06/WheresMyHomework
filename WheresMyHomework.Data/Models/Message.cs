using System.ComponentModel.DataAnnotations;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

public class Message
{
    [Key] public int Id { get; set; }

    [Required] public required DateTime Timestamp { get; set; } = DateTime.Now;
    
    [Required, MaxLength(500)] public required string Content { get; set; }

    [Required] public required ApplicationUser Sender { get; set; }
    [Required] public required ApplicationUser Receiver { get; set; }
}