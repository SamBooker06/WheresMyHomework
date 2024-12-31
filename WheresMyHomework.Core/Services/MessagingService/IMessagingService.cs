namespace WheresMyHomework.Core.Services.MessagingService;

public interface IMessagingService
{
    Task<IEnumerable<MessageResponse>> GetChatHistoryAsync(string correspondentIdOne,
        string correspondentIdTwo, bool oldestFirst = false);

    Task<bool> SendMessageAsync(MessageRequest msgRequest);
}