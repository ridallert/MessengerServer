namespace MessengerServer.Network.Requests
{
    public class GetChatListRequest
    {
        public int UserId { get; set; }

        public GetChatListRequest(int userId)
        {
            UserId = userId;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetChatListRequest),
                Payload = this
            };

            return container;
        }
    }
}
