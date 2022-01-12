﻿using MessengerServer.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Common
{
    public class User
    {
        private string _name;
        private OnlineStatus _isOnline;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }
        public OnlineStatus IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (value != _isOnline)
                {
                    _isOnline = value;
                }
            }
        }

        public User(string name, OnlineStatus isOnline)
        {
            Name = name;
            IsOnline = isOnline;
        }
    }
}
