namespace MessengerServer.Network.Responses
{
    using MessengerServer.DataObjects;
    using System.Collections.Generic;

    public class GetUserListResponse
    {
        public string Result { get; set; }
        public List<User> ContactList { get; set; }

        public GetUserListResponse(string result, List<User> contactList)
        {
            Result = result;
            ContactList = contactList;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListResponse),
                Payload = this
            };

            return container;
        }
    }
}
