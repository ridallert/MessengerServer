namespace MessengerServer.Network.Responses
{
    using MessengerServer.DataObjects;
    using System.Collections.Generic;

    public class GetMessageListResponse
    {
        public string Result { get; set; }
        public List<Message> MessageList { get; set; }

        public GetMessageListResponse(string result, List<Message> messageList)
        {
            Result = result;
            MessageList = messageList;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetMessageListResponse),
                Payload = this
            };

            return container;
        }
    }
}
