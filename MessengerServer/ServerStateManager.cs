using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.Broadcasts;
using MessengerServer.Network.EventArgs;
using MessengerServer.Network.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    public class ServerStateManager
    {
        private DataBaseManager _dataBaseManager;

        public List<User> Users { get; set; }
        public List<Chat> Chats { get; set; }
        private List<LogEntry> EventList { get; set; }

        //public event Action<UserStatusChangedBroadcast> UserStatusChanged;
        public event Action<Chat> NewChatCreated;
        public event Action<Message> MessageReceived;


        public ServerStateManager()
        {
            _dataBaseManager = new DataBaseManager();
            _dataBaseManager.SetConnectionString("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=ServerState; Integrated Security=True;");
            //_dataBaseManager.InitializeDataBase();

            Users = _dataBaseManager.GetUsers();
            Chats = _dataBaseManager.GetChats();
            EventList = _dataBaseManager.GetEventLog();
        }

        public AuthorizationResponse AuthorizeUser(string name)
        {
            bool isUserAlreadyExists = false;
            bool isNameTaken = false;
            int? userId = null;

            foreach (User user in Users)
            {
                if (user.Name == name)
                {
                    isUserAlreadyExists = true;

                    if (user.IsOnline == OnlineStatus.Offline)
                    {
                        user.IsOnline = OnlineStatus.Online;
                        userId = user.UserId;
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
                    return new AuthorizationResponse("NameIsTaken");
                }
                else
                {
                    return new AuthorizationResponse("AlreadyExists", name, userId.Value);
                }
            }
            else
            {
                User newUser = new User(name, OnlineStatus.Online);
                Users.Add(newUser);
                Chats[0].Users.Add(newUser);
                return new AuthorizationResponse("NewUserAdded", name, newUser.UserId);
            }
        }

        public List<Chat> GetChatList(uint userId)
        {
            return Chats.FindAll(chat => chat.Users.Exists(user => user.UserId == userId));
        }

        public CreateNewChatResponse CreateNewChat(string title, List<int> userIdList)
        {
            Chat newChat = new Chat();
            int userCounter = 0;
            foreach (User user in Users)
            {
                foreach (int userId in userIdList)
                {
                    if (user.UserId == userId)
                    {
                        newChat.Users.Add(user);
                        userCounter++;
                    }
                }
            }
            if (userCounter == userIdList.Count)
            {
                if (userIdList.Count > 2)
                {
                    newChat.Title = title;
                }
                Chats.Add(newChat);
                NewChatCreated?.Invoke(newChat);
                return new CreateNewChatResponse("Success");
            }
            else
            {
                return new CreateNewChatResponse("Failure");
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

        public GetUserListResponse GetContacts(string name)
        {
            List<User> contactList = Users.FindAll(user => user.Name != name);

            return new GetUserListResponse("Success", contactList);
        }
        public GetChatListResponse GetChatList(string name)
        {
            List<Chat> chatList = Chats.FindAll(chat => chat.Users.Exists(user => user.Name == name));

            return new GetChatListResponse("Success", chatList);
        }
        public void AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        {
            User sender = Users.Find(user => user.UserId == senderId);
            if (sender != null)
            {

                Chat targetChat = Chats.Find(chat => chat.ChatId == chatId);
                if (targetChat != null)
                {
                    Message message = new Message(sender, targetChat, text, sendTime);
                    targetChat.Messages.Add(message);
                    MessageReceived?.Invoke(message);
                }
                else
                {
                    throw new Exception($"AddMessage: Chat with id = {chatId} does not exist");
                }
            }
            else
            {
                throw new Exception($"AddMessage: Sender with id = {sender} does not exist");
            }
        }

        //public GetPrivateMessageListResponse GetPrivateMessageList(string name)
        //{
        //    List<Message> messages = new List<Message>();

        //    if (Users.Exists(user => user.Name == name))
        //    {
        //        messages = Messages.FindAll(message => message.Sender == name || message.Receiver == name);
        //    }

        //    return new GetPrivateMessageListResponse("Success", messages);
        //}

        //public GetGroupMessageListResponse GetGroupMessageList(string name)
        //{
        //    List<GroupMessage> messages = new List<GroupMessage>();

        //    foreach (Chat chat in Chats)
        //    {
        //        if (chat.Users.Exists(user => user.Name == name))
        //        {
        //            messages.AddRange(GroupMessages.FindAll(message => message.ChatName == chat.Title));
        //        }
        //    }

        //    return new GetGroupMessageListResponse("Success", messages);
        //}

        public GetEventListResponse GetEventLog(DateTime from, DateTime to)
        {
            List<LogEntry> logResponseList = EventList.FindAll(entry => entry.DateTime >= from && entry.DateTime <= to.AddDays(1));

            return new GetEventListResponse("Success", logResponseList);
        }
    }
}
