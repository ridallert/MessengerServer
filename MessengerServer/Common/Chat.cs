﻿namespace MessengerServer.Common
{
    using System.Collections.Generic;
    public class Chat
    {
        public int ChatId { get; set; }
        public string Title { get; set; }
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }

        public Chat(string title, List<User> users) : this() //this(users)
        {
            Title = title;
            Users.AddRange(users);
        }

        //public Chat(List<User> users) : this()
        //{
        //    Users.AddRange(users);
        //}

        public Chat()
        {
            Users = new List<User>();
            Messages = new List<Message>();
        }
    }
}
