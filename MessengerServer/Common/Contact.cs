using MessengerServer.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Common
{
    public class Contact
    {
        public string Title { get; set; }
        public List<string> Users { get; set; }
        public OnlineStatus IsOnline { get; set; }

        //public Dialog(string user)
        //{
        //    ChatName = user;
        //    Users = new List<string>();
        //    Users.Add(user);
        //}
        public Contact(string user)
        {
            Title = user;
            Users = new List<string>();
        }
        public Contact(string user, OnlineStatus isOnline)
        {
            Title = user;
            Users = new List<string>();
            Users.Add(user);
            IsOnline = isOnline;
        }

        [JsonConstructor]
        public Contact(string title, List<string> users)
        {
            Title = title;
            Users = users;
        }

        public bool IsGroop()
        {
            return Users.Count > 1;
        }
    }
}
