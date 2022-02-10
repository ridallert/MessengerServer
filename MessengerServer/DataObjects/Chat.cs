namespace MessengerServer.DataObjects
{
    using System.Collections.Generic;

    public class Chat
    {
        #region Properties

        public int ChatId { get; set; }

        public string Title { get; set; }

        public List<User> Users { get; set; }

        public List<Message> Messages { get; set; }

        #endregion //Properties

        #region Constructors

        public Chat(string title, List<User> users) : this()
        {
            Title = title;
            Users.AddRange(users);
        }

        public Chat()
        {
            Users = new List<User>();
            Messages = new List<Message>();
        }

        #endregion //Constructors
    }
}
