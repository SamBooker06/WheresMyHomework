using Microsoft.EntityFrameworkCore;
using WheresMyHomework.Data;
using WheresMyHomework.Data.Models;

namespace WheresMyHomework.Core.Services.MessagingService;

public class MessagingService(ApplicationDbContext context) : IMessagingService
{
    public async Task<IEnumerable<MessageResponse>> GetChatHistoryAsync(string correspondentIdOne,
        string correspondentIdTwo, bool oldestFirst = false)
    {
        var unorderedMessages = context.Messages.Where(message =>
                message.ReceiverId == correspondentIdOne && message.SenderId == correspondentIdTwo ||
                message.ReceiverId == correspondentIdTwo && message.SenderId == correspondentIdOne)
            .Select(message => new MessageResponse
            {
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                Timestamp = message.Timestamp,
            });

        IEnumerable<MessageResponse> orderedMessages;
        if (oldestFirst)
        {
            orderedMessages = await unorderedMessages.OrderByDescending(msg => msg.Timestamp).ToArrayAsync();
        }
        else
        {
            orderedMessages = await unorderedMessages.OrderBy(msg => msg.Timestamp).ToArrayAsync();
        }
            
        return orderedMessages;
    }

    public async Task<bool> SendMessageAsync(MessageRequest msgRequest)
    {
        if (msgRequest.SenderId is null || msgRequest.ReceiverId is null || msgRequest.Content is null) return false;
        if (msgRequest.SenderId == msgRequest.ReceiverId) return false;

        var message = new Message
        {
            SenderId = msgRequest.SenderId,
            ReceiverId = msgRequest.ReceiverId,
            Content = msgRequest.Content,
            Timestamp = DateTime.Now
        };
        
        await context.Messages.AddAsync(message);
        return await context.SaveChangesAsync() > 0;

    }
}

public class MessageRequest
{
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }

    public string? Content { get; set; }
}

public class MessageResponse
{
    public required DateTime Timestamp { get; init; }

    public required string SenderId { get; init; }
    public required string ReceiverId { get; init; }

    public required string Content { get; init; }
}