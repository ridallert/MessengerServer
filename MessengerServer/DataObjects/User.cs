namespace MessengerServer.DataObjects
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class User
    {
        #region Properties

        public int UserId { get; set; }

        public string Name { get; set; }

        public UserStatus IsOnline { get; set; }

        [JsonIgnore] 
        public List<Chat> Chats { get; set; }

        #endregion //Properties

        #region Constructors

        [JsonConstructor]
        public User(int userId, string name, UserStatus isOnline, List<Chat> chats) : this(name, isOnline)
        {
            UserId = userId;
            Chats = chats;
        }

        public User(string name, UserStatus isOnline)
        {
            Name = name;
            IsOnline = isOnline;
            Chats = new List<Chat>();
        }

        public User()
        { 

        }

        #endregion //Constructors
    }
}
