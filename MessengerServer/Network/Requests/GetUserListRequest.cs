﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.Requests
{
    class GetUserListRequest
    {
        public string Name { get; set; }

        public GetUserListRequest(string name)
        {
            Name = name;
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
