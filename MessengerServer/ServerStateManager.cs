using MessengerServer.Common;
using MessengerServer.Network;
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
        public List<Message> Messages { get; set; }
        private List<LogEntry> EventList { get; set; }

        public event Action<Chat> NewChatCreated;
        //public event Action UserListChanged;
        //public event Action<Message> MessageReceived;

        public ServerStateManager()
        {
            _dataBaseManager = new DataBaseManager();

            Users = new List<User>();
            Chats = new List<Chat>();
            //Messages = new List<Message>();
            EventList = new List<LogEntry>();

            //_repository.Create();   AttachDbFilename='|DataDirectory|\Bookstore.mdf'
            //.\SQLEXPRESS;AttachDbFilename='|DataDirectory|\Bookstore.mdf';
            //string str = AppDomain.CurrentDomain.BaseDirectory;

            _dataBaseManager.Connect("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=ServerState; Integrated Security=True;");

            //DB INITIALIZATION

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Offline));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));

            Chats.Add(new Chat("Public chat", Users));

            Chats[0].Messages.Add(new Message(Users[0], Chats[0].ChatId, Chats[0], $"Привет всем от {Users[0].Name}!", DateTime.Now));
            Chats[0].Messages.Add(new Message(Users[1], Chats[0].ChatId, Chats[0], $"Привет всем от {Users[1].Name}!", DateTime.Now));
            Chats[0].Messages.Add(new Message(Users[2], Chats[0].ChatId, Chats[0], $"Привет всем от {Users[2].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[0], Users[2]));
            Chats[1].Messages.Add(new Message(Users[0], Chats[1].ChatId, Chats[1], $"{Users[2].Name}, привет от {Users[0].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[1], Users[2]));
            Chats[2].Messages.Add(new Message(Users[1], Chats[2].ChatId, Chats[2], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[3], Users[2]));
            Chats[3].Messages.Add(new Message(Users[3], Chats[2].ChatId, Chats[3], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

            Chats.Add(new Chat("ЕвгЯкРид", new List<User> { Users[0], Users[1], Users[4] }));
            Chats[4].Messages.Add(new Message(Users[4], Chats[4].ChatId, Chats[4], Users[0].Name + ", " + Users[1].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));

            Chats.Add(new Chat(Users[0], Users[4]));
            Chats[5].Messages.Add(new Message(Users[0], Chats[5].ChatId, Chats[5], $"{Users[4].Name}, привет от {Users[0].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[1], Users[4]));
            Chats[6].Messages.Add(new Message(Users[1], Chats[6].ChatId, Chats[6], $"{Users[4].Name}, привет от {Users[1].Name}!", DateTime.Now));
            Chats[6].Messages.Add(new Message(Users[4], Chats[6].ChatId, Chats[6], $"{Users[1].Name}, привет от {Users[4].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[2], Users[4]));
            Chats[7].Messages.Add(new Message(Users[2], Chats[7].ChatId, Chats[7], $"{Users[4].Name}, привет от {Users[2].Name}!", DateTime.Now));
            Chats[7].Messages.Add(new Message(Users[4], Chats[7].ChatId, Chats[7], $"{Users[2].Name}, привет от {Users[4].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[3], Users[4]));
            Chats[8].Messages.Add(new Message(Users[3], Chats[8].ChatId, Chats[8], $"{Users[4].Name}, привет от {Users[3].Name}!", DateTime.Now));
            Chats[8].Messages.Add(new Message(Users[4], Chats[8].ChatId, Chats[8], $"{Users[3].Name}, привет от {Users[4].Name}!", DateTime.Now));

            DateTime startDate = new DateTime(2022, 1, 12, 16, 45, 58);

            foreach (User user in Users)
            {
                startDate = startDate.AddDays(1);
                EventList.Add(new LogEntry(EventType.Event, user.Name + " is joined", startDate));
            }
            foreach (Chat chat in Chats)
            {
                if (chat.Users.Count > 2)
                {
                    foreach (Message message in chat.Messages)
                    {
                        EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message in '{chat.Title}' group chat", message.SendTime));
                    }
                }
                else
                {
                    foreach (Message message in chat.Messages)
                    {
                        EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message to {chat.Users.Find(user => user.Name != message.Sender.Name).Name}", message.SendTime));
                    }
                }
            }
            //foreach (Chat chat in Chats)
            //{
            //    Messages.AddRange(chat.Messages);
            //}

            //_dataBaseManager.AddUsers(Users);
            //_dataBaseManager.AddChats(Chats);
            //foreach (Chat chat in Chats)
            //{
            //    _dataBaseManager.AddMessages(Messages);
            //}

            //_dataBaseManager.AddLogEntries(EventList);

            //DB INITIALIZATION END

            //Users = _dataBaseManager.GetUsers();
            //GroupChats = _dataBaseManager.GetGroupChats();
            //PrivateMessages = _dataBaseManager.GetPrivateMessages();
            //GroupMessages = _dataBaseManager.GetGroupMessages();
            //EventList = _dataBaseManager.GetEventLog();
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

        //public Chat StartPrivateChat(int senderId, int receiverId, Message message)
        //{
        //    Chat targetChat = Chats.Find(chat => chat.Users.Count == 2 &&
        //                                         chat.Users.Exists(user => user.UserId == senderId) &&
        //                                         chat.Users.Exists(user => user.UserId == receiverId));

        //    if (targetChat == null)
        //    {
        //        targetChat = new Chat(Users.Find(user => user.UserId == senderId), Users.Find(user => user.UserId == receiverId));
        //        Chats.Add(targetChat);
        //    }

        //    targetChat.Messages.Add(message);

        //    return targetChat;
        //}


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
                    //UserListChanged?.Invoke();
                    return new AuthorizationResponse("AlreadyExists", name, userId);
                }
            }
            else
            {
                User newUser = new User(name, OnlineStatus.Online);
                Users.Add(newUser);
                Chats[0].Users.Add(newUser);
                //UserListChanged?.Invoke();
                return new AuthorizationResponse("NewUserAdded", name, newUser.UserId);
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

        public GetContactsResponse GetContacts(string name)
        {
            List<User> contactList = Users.FindAll(user => user.Name != name);

            return new GetContactsResponse("Success", contactList);
        }
        public GetChatListResponse GetChatList(string name)
        {
            List<Chat> chatList = Chats.FindAll(chat => chat.Users.Exists(user => user.Name == name));

            return new GetChatListResponse("Success", chatList);
        }
        //public void AddPrivateMessage(string sender, string receiver, string text, DateTime sendTime)
        //{
        //    if (Users.Exists(user => user.Name == sender))
        //    {
        //        if (Users.Exists(user => user.Name == receiver))
        //        {
        //            Message message = new Message(sender, receiver, text, sendTime);
        //            Messages.Add(message);
        //            PrivateMessageReceived?.Invoke(message);
        //        }
        //        else
        //        {
        //            throw new Exception("AddPrivateMessage: Sender '" + receiver + "' does not exist");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("AddPrivateMessage: Sender '" + sender + "' does not exist");
        //    }
        //}

        //public void AddGroupMessage(string sender, string chatName, string text, DateTime sendTime)
        //{
        //    if (Users.Exists(user => user.Name == sender))
        //    {
        //        Chat groupChat = Chats.Find(chat => chat.Title == chatName);
        //        if (groupChat != null)
        //        {
        //            GroupMessage message = new GroupMessage(sender, chatName, text, sendTime);
        //            GroupMessages.Add(message);
        //            GroupMessageReceived?.Invoke(message, groupChat);
        //        }
        //        else
        //        {
        //            throw new Exception("AddGroupMessage: GropChat '" + chatName + "' does not exist");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("AddGroupMessage: Sender '" + sender + "' does not exist");
        //    }
        //}

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
