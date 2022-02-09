namespace MessengerServer.Network.Requests
{
    class AuthorizationRequest
    {
        public string Name { get; set; }

        public AuthorizationRequest(string name)
        {
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(AuthorizationRequest),
                Payload = this
            };

            return container;
        }
    }
}
