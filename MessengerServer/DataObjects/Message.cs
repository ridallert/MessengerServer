namespace MessengerServer.DataObjects
{
    using System;

    using Newtonsoft.Json;

    public class Message
    {
        #region Properties

        public int MessageId { get; set; }

        [JsonIgnore]
        public User Sender { get; set; }

        [JsonIgnore]
        public Chat Chat { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public DateTime SendTime { get; set; }

        #endregion //Properties

        #region Constructors

        public Message(User sender, Chat chat, string text, DateTime sendTime)
        {
            Sender = sender;
            Chat = chat;
            SenderName = sender.Name;
            Text = text;
            SendTime = sendTime;
        }

        public Message()
        {

        }

        #endregion //Constructors
    }
}
