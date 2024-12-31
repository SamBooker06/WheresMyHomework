using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WheresMyHomework.Data.Models.Users;

namespace WheresMyHomework.Data.Models;

public class Message
{
    [Key] public int Id { get; set; }

    [Required] public required DateTime Timestamp { get; set; } = DateTime.Now;

    [Required, MaxLength(500)] public required string Content { get; set; }

    [Required] public ApplicationUser? Sender { get; set; }

    [ForeignKey("SenderId"), MaxLength(36)]
    public required string SenderId { get; set; }

    [Required] public ApplicationUser? Receiver { get; set; }

    [ForeignKey("ReceiverId"), MaxLength(36)]
    public required string ReceiverId { get; set; }
}