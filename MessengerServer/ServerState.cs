using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.EventArgs;
using MessengerServer.Network.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    public class ServerState
    {
        private List<User> _users;

        public List<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                UserListChanged?.Invoke();
            }
        }

        public event Action UserListChanged;
        public event EventHandler<Message> PrivateMessageReceived;

        public ServerState()
        {
            Users = new List<User>();

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Online));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));

            Users[0].MessageList.Add(new Message(Users[0].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
            Users[1].MessageList.Add(new Message(Users[1].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[1].Name + "!", DateTime.Now));
            Users[3].MessageList.Add(new Message(Users[3].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Users[4].MessageList.Add(new Message(Users[4].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));
            Users[2].MessageList.Add(new Message(Users[2].Name, Users[1].Name, Users[1].Name + ", Привет от " + Users[2].Name + "!", DateTime.Now));
            Users[3].MessageList.Add(new Message(Users[3].Name, Users[4].Name, Users[4].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Users[0].MessageList.Add(new Message(Users[0].Name, Users[3].Name, Users[3].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));

            Users[2].MessageList.Add(new Message(Users[0].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
            Users[2].MessageList.Add(new Message(Users[1].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[1].Name + "!", DateTime.Now));
            Users[2].MessageList.Add(new Message(Users[3].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Users[2].MessageList.Add(new Message(Users[4].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));
            Users[1].MessageList.Add(new Message(Users[2].Name, Users[1].Name, Users[1].Name + ", Привет от " + Users[2].Name + "!", DateTime.Now));
            Users[4].MessageList.Add(new Message(Users[3].Name, Users[4].Name, Users[4].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Users[3].MessageList.Add(new Message(Users[0].Name, Users[3].Name, Users[3].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
        }

        public GetContactsResponse GetContacts(string name)
        {
            List<User> contactList = new List<User>();
            foreach (User user in Users)
            {
                if (name != null && name != user.Name)
                {
                    contactList.Add(new User(user.Name, user.IsOnline));
                }
            }

            if (contactList.Count != 0)
            {
                return new GetContactsResponse("Success", contactList);
            }
            else
            {
                return new GetContactsResponse("Empty", new List<User>());
            }
        }

        public void AddPrivateMessage(string sender, string receiver, string text, DateTime sendTime)
        {
            Message message = new Message(sender, receiver, text, sendTime);

            foreach (User user in Users)
            {
                if (user.Name == sender || user.Name == receiver)
                {
                    user.MessageList.Add(message);
                }
            }
            PrivateMessageReceived?.Invoke(this, message);
        }

        public void SendGroupMessage(string sender, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                //Users[i].MessageList.Add(new Message(sender, Users[i].Name, text, true));
            }
        }

        public GetPrivateMessageListResponse GetPrivateMessageList(string name)
        {
            List<Message> messages = new List<Message>();

            foreach (User user in Users)
            {
                if (user.Name == name)
                {
                    foreach (Message message in user.MessageList)
                    {
                        if(!message.IsGroopChatMessage)
                            messages.Add(message);
                    }
                }
            }

            if (messages.Count != 0)
            {
                return new GetPrivateMessageListResponse("Success", messages);
            }
            else
            {
                return new GetPrivateMessageListResponse("Empty", messages);
            }
        }
        //public GetPublicMessageListResponse GetPublicMessageList(string name)
        //{
        //    List<Message> messages = new List<Message>();

        //    foreach (User user in Users)
        //    {
        //        if (user.Name == name)
        //        {
        //            foreach (Message message in user.MessageList)
        //            {
        //                if (!message.IsGroopChatMessage)
        //                    messages.Add(message);
        //            }
        //        }
        //    }

        //    if (messages.Count != 0)
        //    {
        //        return new GetPublicMessageListResponse("Success", messages);
        //    }
        //    else
        //    {
        //        return new GetPublicMessageListResponse("Empty", messages);
        //    }
        //}

        //public List<Message> GetPublicMessageList(User me)
        //{
        //    List<Message> messages = new List<Message>();

        //    for (int i = 0; i < me.MessageList.Count; i++)
        //    {
        //        if (me.MessageList[i].IsGroopChatMessage == true)
        //        {
        //            messages.Add(me.MessageList[i]);
        //        }
        //    }

        //    return messages;
        //}
        public AuthorizationResponse AuthorizeUser(string name)
        {
            bool isUserAlreadyExists = false;
            bool isNameTaken = false;

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name == name)
                {
                    isUserAlreadyExists = true;

                    if (Users[i].IsOnline == OnlineStatus.Offline)
                    {
                        Users[i].IsOnline = OnlineStatus.Online;
                    }
                    else
                    {
                        isNameTaken = true;
                    }
                }
            }

            if (isUserAlreadyExists)
            {
                if (isNameTaken)
                {
                    return new AuthorizationResponse("NameIsTaken", name);
                }
                else
                {
                    return new AuthorizationResponse("AlreadyExists", name);
                }
            }
            else
            {
                Users.Add(new User(name, OnlineStatus.Online));
                return new AuthorizationResponse("NewUserAdded", name);
            }
        }

        
        public void SetUserOffline(string name)
        {
            foreach (User user in Users)
            {
                if (user.Name == name)
                {
                    user.IsOnline = OnlineStatus.Offline;
                }
            }
        }
    }
}
