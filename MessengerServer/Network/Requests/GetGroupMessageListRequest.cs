using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.Requests
{
    public class GetGroupMessageListRequest
    {
        public string Name { get; set; }

        public GetGroupMessageListRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetGroupMessageListRequest),
                Payload = this
            };

            return container;
        }
    }
}
