using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.Requests
{
    class SendGroupMessageRequest
    {
        public string Sender { get; set; }
        public string ChatName { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public SendGroupMessageRequest(string sender, string chatName, string text, DateTime sendTime)
        {
            Sender = sender;
            ChatName = chatName;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendGroupMessageRequest),
                Payload = this
            };

            return container;
        }
    }
}
