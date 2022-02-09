namespace MessengerServer.Network.Broadcasts
{
    using MessengerServer.DataObjects;
    public class UserStatusChangedBroadcast
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public OnlineStatus Status { get; set; }

        public UserStatusChangedBroadcast(string name, int userId, OnlineStatus status)
        {
            Name = name;
            UserId = userId;
            Status = status;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(UserStatusChangedBroadcast),
                Payload = this
            };

            return container;
        }
    }
}
