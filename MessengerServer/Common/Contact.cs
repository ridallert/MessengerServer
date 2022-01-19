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
        private static int _idCounter;
        public int ContactId { get; set; }
        public string Title { get; set; }
        public List<string> Users { get; set; }
        public OnlineStatus IsOnline { get; set; }

        public Contact(string user)
        {
            ContactId = _idCounter++;
            Title = user;
            Users = new List<string>();
        }
        public Contact(string user, OnlineStatus isOnline)
        {
            ContactId = _idCounter++;
            Title = user;
            Users = new List<string>();
            Users.Add(user);
            IsOnline = isOnline;
        }
        //public Contact(int contactId, string user, OnlineStatus isOnline)
        //{
        //    ContactId = contactId;
        //    Title = user;
        //    Users = new List<string>();
        //    Users.Add(user);
        //    IsOnline = isOnline;
        //}
        //[JsonConstructor]
        //public Contact(int contactId, string title, List<string> users)
        //{
        //    ContactId = contactId;
        //    Title = title;
        //    Users = users;
        //}

        public bool IsGroop()
        {
            return Users.Count > 1;
        }
    }
}
