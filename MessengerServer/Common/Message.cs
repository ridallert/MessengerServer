namespace MessengerServer.Common
{
    using Newtonsoft.Json;
    using System;
    public class Message
    {
        public int MessageId { get; set; }
        [JsonIgnore]
        public User Sender { get; set; }
        [JsonIgnore]
        public Chat Chat { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        //[JsonConstructor]
        //public Message(int messageId, User sender, Chat chat, string text, DateTime sendTime) : this(sender, chat, text, sendTime)
        //{
        //    MessageId = messageId;
        //}
        public Message(User sender, Chat chat, string text, DateTime sendTime) : this()
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
    }
}
