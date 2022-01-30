namespace MessengerServer.Common
{
    using System.Collections.Generic;
    public class GroupChat : Chat
    {
        public string Title { get; set; }

        public GroupChat(string title) : base()
        {
            Title = title;
        }
        public GroupChat(string title, List<User> users) : base()
        {
            Title = title;
            Users.AddRange(users);
        }
        public GroupChat() : base() { }

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
    }
}
