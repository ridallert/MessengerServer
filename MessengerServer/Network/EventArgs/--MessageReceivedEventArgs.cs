using MessengerServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.EventArgs
{
    public class MessageReceivedEventArgs
    {
        public string ClientName { get; }
        public Message Message { get; }

        public MessageReceivedEventArgs(string clientName, Message message)
        {
            ClientName = clientName;
            Message = message;
        }
    }
}
