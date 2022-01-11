﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Network.EventArgs
{
    public class UserStatusChangedEventArgs
    {
        public string Name { get; }
        public OnlineStatus Status { get; }

        public UserStatusChangedEventArgs(string name, OnlineStatus status)
        {
            Name = name;
            Status = status;
        }
    }
}
