﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.Requests
{
    public class GetPublicMessageListRequest
    {
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetPublicMessageListRequest),
                Payload = this
            };

            return container;
        }
    }
}
