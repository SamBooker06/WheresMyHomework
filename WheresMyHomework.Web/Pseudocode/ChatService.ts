class Message {
    public MessageId: number;
    public SenderId: string;
    public RecipientId: string;
    
    public Content: string
    public Timestamp: Date
}

class ChatService {
    public GetChat(userOneId: string, userTwoId: string): Message[] {}
    public SendMessage(senderId: string, recipientId: string, content: string): void
}