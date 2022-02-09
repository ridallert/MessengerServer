namespace MessengerServer.Network.Requests
{
    using System;
    class GetEventListRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public GetEventListRequest(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListRequest),
                Payload = this
            };

            return container;
        }
    }
}
