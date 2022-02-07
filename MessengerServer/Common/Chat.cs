namespace MessengerServer.Common
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class Chat
    {
        private static int _idCounter;
        public int ChatId { get; set; }
        public string Title { get; set; }
        public List<User> Users { get; set; }
        //[JsonIgnore]
        public List<Message> Messages { get; set; }

        public Chat(string title, List<User> users) : this(users)
        {
            Title = title;
        }

        public Chat(List<User> users) : this()
        {
            Users.AddRange(users);
        }
        public Chat(User userA, User userB) : this()
        {
            Users.Add(userA);
            Users.Add(userB);
        }
        public Chat()
        {
            ChatId = ++_idCounter;
            Users = new List<User>();
            Messages = new List<Message>();
        }
    }
}
