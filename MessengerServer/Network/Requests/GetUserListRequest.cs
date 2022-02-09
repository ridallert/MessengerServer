namespace MessengerServer.Network.Requests
{
    class GetUserListRequest
    {
        public int UserId { get; set; }

        public GetUserListRequest(int userId)
        {
            UserId = userId;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListRequest),
                Payload = this
            };

            return container;
        }
    }
}
