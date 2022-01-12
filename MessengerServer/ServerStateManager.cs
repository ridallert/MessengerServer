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
    public class ServerStateManager
    {
        private List<User> _users;
        private List<Message> _messages;
        private List<Chat> _chats;
        public List<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                UserListChanged?.Invoke();
            }
        }
        public List<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
            }
        }
        public List<Chat> Chats
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

        public ServerStateManager()
        {
            Users = new List<User>();

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Online));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));


            Messages = new List<Message>();
            Messages.Add(new Message(Users[0].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[1].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[1].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[3].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[4].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[2].Name, Users[1].Name, Users[1].Name + ", Привет от " + Users[2].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[3].Name, Users[4].Name, Users[4].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[0].Name, Users[3].Name, Users[3].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));

            Messages.Add(new Message(Users[0].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[1].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[1].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[3].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[4].Name, Users[2].Name, Users[2].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[2].Name, Users[1].Name, Users[1].Name + ", Привет от " + Users[2].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[3].Name, Users[4].Name, Users[4].Name + ", Привет от " + Users[3].Name + "!", DateTime.Now));
            Messages.Add(new Message(Users[0].Name, Users[3].Name, Users[3].Name + ", Привет от " + Users[0].Name + "!", DateTime.Now));
        }

        public GetContactsResponse GetContacts(string name)
        {
            List<User> contactList = new List<User>();
            contactList = Users.FindAll(user => name != null && name != user.Name);

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
            if (Users.Exists(user => user.Name == sender))
            {
                if (Users.Exists(user => user.Name == receiver))
                {
                    Message message = new Message(sender, receiver, text, sendTime);
                    Messages.Add(message);
                    PrivateMessageReceived?.Invoke(this, message);
                }
                else
                {
                    throw new Exception("AddPrivateMessage: Sender '" + receiver + "' does not exist");
                }
            }
            else
            {
                throw new Exception("AddPrivateMessage: Sender '" + sender + "' does not exist");
            }
        }

        //public void AddPrivateMessage(string sender, string text)
        //{
        //    for (int i = 0; i < Users.Count; i++)
        //    {
        //        //Users[i].MessageList.Add(new Message(sender, Users[i].Name, text, true));
        //    }
        //}

        public GetPrivateMessageListResponse GetPrivateMessageList(string name)
        {
            List<Message> messages = new List<Message>();

            foreach (User user in Users)
            {
                foreach (Message message in Messages)
                {
                    if ((message.Sender == name || message.Receiver == name) && !message.IsGroopChatMessage)
                    {
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
