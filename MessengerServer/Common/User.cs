namespace MessengerServer.Common
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class User
    {
        private static int _idCounter;
        public int UserId { get; set; }
        public string Name { get; set; }
        public OnlineStatus IsOnline { get; set; }
        [JsonIgnore] 
        public List<Chat> Chats { get; set; }

        [JsonConstructor]
        public User(int userId, string name, OnlineStatus isOnline, List<Chat> chats) : this(name, isOnline)
        {
            UserId = userId;
            Chats = chats;
        }
        public User(string name, OnlineStatus isOnline) : this()
        {
            Name = name;
            IsOnline = isOnline;
        }
        public User()
        {
            UserId = ++_idCounter;
            Chats = new List<Chat>();
        }
    }
}
