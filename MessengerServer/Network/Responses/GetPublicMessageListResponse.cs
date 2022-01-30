using MessengerServer.Common;
using System.Collections.Generic;

namespace MessengerServer.Network.Responses
{
    public class GetGroupMessageListResponse
    {
        public string Result { get; set; }
        public List<Message> MessageList { get; set; }

        public GetGroupMessageListResponse(string result, List<Message> messageList)
        {
            Result = result;
            MessageList = messageList;
        }

        //public GetPublicMessageListResponce(string result)
        //{
        //    Result = result;
        //    MessageList = new List<Message>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetGroupMessageListResponse),
                Payload = this
            };

            return container;
        }
    }
}
