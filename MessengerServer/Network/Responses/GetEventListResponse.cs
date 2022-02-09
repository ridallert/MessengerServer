namespace MessengerServer.Network.Responses
{
    using System.Collections.Generic;
    using MessengerServer.DataObjects;

    public class GetEventListResponse
    {
        public string Result { get; set; }
        public List<LogEntry> EventList {get;set;}

        public GetEventListResponse(string result, List<LogEntry> eventList)
        {
            Result = result;
            EventList = eventList;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListResponse),
                Payload = this
            };

            return container;
        }
    }
}
