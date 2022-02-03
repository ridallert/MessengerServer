﻿using MessengerServer.Common;
using System.Collections.Generic;


namespace MessengerServer.Network.Responses
{
    public class GetUserListResponse
    {
        public string Result { get; set; }
        public List<User> ContactList { get; set; }

        public GetUserListResponse(string result, List<User> contactList)
        {
            Result = result;
            ContactList = contactList;
        }

        //public GetContactsResponse(string result)
        //{
        //    Result = result;
        //    UserList = new List<User>();
        //}

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
