namespace MessengerServer.Common
{
    using System.Collections.Generic;
    public class IChat
    {
        public int ChatId { get; set; }
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }
    }
}