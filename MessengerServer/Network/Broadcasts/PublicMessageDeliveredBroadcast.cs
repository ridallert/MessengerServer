namespace MessengerServer.Network.Broadcasts
{
    using System;

    class PublicMessageDeliveredBroadcast
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public PublicMessageDeliveredBroadcast(string sender, string text, DateTime sendTime)
        {
            Sender = sender;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(PublicMessageDeliveredBroadcast),
                Payload = this
            };

            return container;
        }
    }
}
