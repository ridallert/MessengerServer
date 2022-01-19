using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Common
{
    public class Message
    {
        private static int _idCounter;
        public int MessageId { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public Message(string sender, string receiver, string text, DateTime sendTime)
        {
            MessageId = _idCounter++;
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendTime = sendTime;
        }
    }
}
