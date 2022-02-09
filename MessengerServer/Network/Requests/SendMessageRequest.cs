namespace MessengerServer.Network.Requests
{
    using System;

    class SendMessageRequest
    {
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public SendMessageRequest(int senderId, int chatId, string text, DateTime sendTime)
        {
            SenderId = senderId;
            ChatId = chatId;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendMessageRequest),
                Payload = this
            };

            return container;
        }
    }
}
