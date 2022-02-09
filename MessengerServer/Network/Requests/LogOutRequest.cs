namespace MessengerServer.Network.Requests
{
    class LogOutRequest
    {
        public string Name { get; set; }

        public LogOutRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(LogOutRequest),
                Payload = this
            };

            return container;
        }
    }
}
