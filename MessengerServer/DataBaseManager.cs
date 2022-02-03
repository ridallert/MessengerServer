namespace MessengerServer
{
    using MessengerServer.Common;
    using MessengerServer.Network.Responses;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class DataBaseManager
    {
        //private DataBaseContext _dataBase;
        private string _connectionString;
        //public void Connect(string connectionString)
        //{
        //    try
        //    {
        //        _dataBase = new DataBaseContext(connectionString);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        Console.ReadLine();
        //    }
        //}
        public void SetConnectionString(string connString)
        {
            _connectionString = connString;
        }
        public bool IsDataBaseExists()
        {
            return _dataBase.Database.Exists();
        }

        public void InitializeDataBase()
        {
            List<User> Users = new List<User>();
            List<Chat> Chats = new List<Chat>();
            List<LogEntry> EventList = new List<LogEntry>();

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Offline));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));

            Chats.Add(new Chat("Public chat", Users));

            Chats[0].Messages.Add(new Message(Users[0], Chats[0], $"Привет всем от {Users[0].Name}!", DateTime.Now));
            Chats[0].Messages.Add(new Message(Users[1], Chats[0], $"Привет всем от {Users[1].Name}!", DateTime.Now));
            Chats[0].Messages.Add(new Message(Users[2], Chats[0], $"Привет всем от {Users[2].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[0], Users[2]));
            Chats[1].Messages.Add(new Message(Users[0], Chats[1], $"{Users[2].Name}, привет от {Users[0].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[1], Users[2]));
            Chats[2].Messages.Add(new Message(Users[1], Chats[2], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[3], Users[2]));
            Chats[3].Messages.Add(new Message(Users[3], Chats[3], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

            Chats.Add(new Chat("ЕвгЯкРид", new List<User> { Users[0], Users[1], Users[4] }));
            Chats[4].Messages.Add(new Message(Users[4], Chats[4], Users[0].Name + ", " + Users[1].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));

            Chats.Add(new Chat(Users[0], Users[4]));
            Chats[5].Messages.Add(new Message(Users[0], Chats[5], $"{Users[4].Name}, привет от {Users[0].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[1], Users[4]));
            Chats[6].Messages.Add(new Message(Users[1], Chats[6], $"{Users[4].Name}, привет от {Users[1].Name}!", DateTime.Now));
            Chats[6].Messages.Add(new Message(Users[4], Chats[6], $"{Users[1].Name}, привет от {Users[4].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[2], Users[4]));
            Chats[7].Messages.Add(new Message(Users[2], Chats[7], $"{Users[4].Name}, привет от {Users[2].Name}!", DateTime.Now));
            Chats[7].Messages.Add(new Message(Users[4], Chats[7], $"{Users[2].Name}, привет от {Users[4].Name}!", DateTime.Now));

            Chats.Add(new Chat(Users[3], Users[4]));
            Chats[8].Messages.Add(new Message(Users[3], Chats[8], $"{Users[4].Name}, привет от {Users[3].Name}!", DateTime.Now));
            Chats[8].Messages.Add(new Message(Users[4], Chats[8], $"{Users[3].Name}, привет от {Users[4].Name}!", DateTime.Now));

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

            AddUsers(Users);
            AddChats(Chats);
            AddLogEntries(EventList);
        }

        public AuthorizationResponse AuthorizeUser(string name)
        {
            using (DataBaseContext dataBase = new DataBaseContext())
            {
                // получаем первый объект
                Phone p1 = db.Phones.FirstOrDefault();

                p1.Price = 30000;
                db.SaveChanges();   // сохраняем изменения
            }

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
                //Users.Add(newUser);
                //Chats[0].Users.Add(newUser);
                return new AuthorizationResponse("NewUserAdded", name, newUser.UserId);
            }
        }
        public List<User> GetUsers()
        {
            List<User> userList = new List<User>();

            foreach (User user in _dataBase.Users.Include(user => user.Chats))
            {
                userList.Add(user);
            }
            return userList;
        }
        public List<Chat> GetChats()
        {
            List<Chat> chatList = new List<Chat>();

            foreach (Chat chat in _dataBase.Chats.Include(chat => chat.Users).Include(chat => chat.Messages))
            {
                chatList.Add(chat);
            }
            return chatList;
        }
        public List<Message> GetPrivateMessages()
        {
            List<Message> messageList = new List<Message>();

            foreach (Message message in _dataBase.Messages)
            {
                messageList.Add(message);
            }

            return messageList;
        }
        public List<Message> GetGroupMessages()
        {
            List<Message> messageList = new List<Message>();

            foreach (Message message in _dataBase.Messages)
            {
                messageList.Add(message);
            }

            return messageList;
        }
        public List<LogEntry> GetEventLog()
        {
            List<LogEntry> eventList = new List<LogEntry>();

            foreach (LogEntry entry in _dataBase.EventList)
            {
                eventList.Add(entry);
            }
            return eventList;
        }
        public void AddUser(User user)
        {
            _dataBase.Users.Add(user);
            _dataBase.SaveChanges();
        }
        public void AddUsers(List<User> users)
        {
            _dataBase.Users.AddRange(users);
            _dataBase.SaveChanges();
        }
        public void AddChat(Chat chat)
        {
            _dataBase.Chats.Add(chat);
            _dataBase.SaveChanges();
        }
        public void AddChats(List<Chat> chats)
        {
            _dataBase.Chats.AddRange(chats);
            _dataBase.SaveChanges();
        }
        public void AddMessage(Message message)
        {
            _dataBase.Messages.Add(message);
            _dataBase.SaveChanges();
        }
        public void AddMessages(List<Message> messages)
        {
            _dataBase.Messages.AddRange(messages);
            _dataBase.SaveChanges();
        }
        public void AddLogEntry(LogEntry entry)
        {
            _dataBase.EventList.Add(entry);
            _dataBase.SaveChanges();
        }
        public void AddLogEntries(List<LogEntry> entries)
        {
            _dataBase.EventList.AddRange(entries);
            _dataBase.SaveChanges();
        }
    }
}
