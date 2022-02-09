namespace MessengerServer.Network.Responses
{
    using MessengerServer.DataObjects;

    public class NewChatCreatedResponse
    {
        public Chat Chat { get; set; }

        public NewChatCreatedResponse(Chat chat)
        {
            Chat = chat;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(NewChatCreatedResponse),
                Payload = this
            };

            return container;
        }
    }
}
