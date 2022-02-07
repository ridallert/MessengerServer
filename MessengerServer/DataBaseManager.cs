namespace MessengerServer
{
    using MessengerServer.Common;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using MessengerServer.Configurations;
    using MessengerServer.Network.Broadcasts;
    using MessengerServer.Network.Responses;

    public class DataBaseManager
    {
        private string _connectionString;

        public event Action<UserStatusChangedBroadcast> UserStatusChanged;
        public event Action<Chat> NewChatCreated;
        public event Action<Message> MessageReceived;

        public DataBaseManager()
        {
            var configManager = new ConfigManager();
            _connectionString = configManager.ConnectionSettings.ToString();

            try
            {
                DataBaseContext _dataBase = new DataBaseContext(_connectionString);
                _dataBase?.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Database connection error:\n" + e.Message);
                _connectionString = configManager.GetDefaultConnectionString().ToString();

                DataBaseContext _dataBase = new DataBaseContext(_connectionString);
                _dataBase?.Dispose();
            }
            ResetOnlineStatus();
        }

        //public void InitializeDataBase()
        //{
        //    List<User> Users = new List<User>();
        //    List<Chat> Chats = new List<Chat>();
        //    List<LogEntry> EventList = new List<LogEntry>();

        //    Users.Add(new User("Евгений", OnlineStatus.Offline));
        //    Users.Add(new User("Яков", OnlineStatus.Offline));
        //    Users.Add(new User("Виктория", OnlineStatus.Offline));
        //    Users.Add(new User("Мария", OnlineStatus.Offline));
        //    Users.Add(new User("Ридаль", OnlineStatus.Offline));

        //    Chats.Add(new Chat("Public chat", Users));

        //    Chats[0].Messages.Add(new Message(Users[0], Chats[0], $"Привет всем от {Users[0].Name}!", DateTime.Now));
        //    Chats[0].Messages.Add(new Message(Users[1], Chats[0], $"Привет всем от {Users[1].Name}!", DateTime.Now));
        //    Chats[0].Messages.Add(new Message(Users[2], Chats[0], $"Привет всем от {Users[2].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[0], Users[2]));
        //    Chats[1].Messages.Add(new Message(Users[0], Chats[1], $"{Users[2].Name}, привет от {Users[0].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[1], Users[2]));
        //    Chats[2].Messages.Add(new Message(Users[1], Chats[2], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[3], Users[2]));
        //    Chats[3].Messages.Add(new Message(Users[3], Chats[3], $"{Users[2].Name}, привет от {Users[1].Name}!", DateTime.Now));

        //    Chats.Add(new Chat("ЕвгЯкРид", new List<User> { Users[0], Users[1], Users[4] }));
        //    Chats[4].Messages.Add(new Message(Users[4], Chats[4], Users[0].Name + ", " + Users[1].Name + ", Привет от " + Users[4].Name + "!", DateTime.Now));

        //    Chats.Add(new Chat(Users[0], Users[4]));
        //    Chats[5].Messages.Add(new Message(Users[0], Chats[5], $"{Users[4].Name}, привет от {Users[0].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[1], Users[4]));
        //    Chats[6].Messages.Add(new Message(Users[1], Chats[6], $"{Users[4].Name}, привет от {Users[1].Name}!", DateTime.Now));
        //    Chats[6].Messages.Add(new Message(Users[4], Chats[6], $"{Users[1].Name}, привет от {Users[4].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[2], Users[4]));
        //    Chats[7].Messages.Add(new Message(Users[2], Chats[7], $"{Users[4].Name}, привет от {Users[2].Name}!", DateTime.Now));
        //    Chats[7].Messages.Add(new Message(Users[4], Chats[7], $"{Users[2].Name}, привет от {Users[4].Name}!", DateTime.Now));

        //    Chats.Add(new Chat(Users[3], Users[4]));
        //    Chats[8].Messages.Add(new Message(Users[3], Chats[8], $"{Users[4].Name}, привет от {Users[3].Name}!", DateTime.Now));
        //    Chats[8].Messages.Add(new Message(Users[4], Chats[8], $"{Users[3].Name}, привет от {Users[4].Name}!", DateTime.Now));

        //    DateTime startDate = new DateTime(2022, 1, 12, 16, 45, 58);

        //    foreach (User user in Users)
        //    {
        //        startDate = startDate.AddDays(1);
        //        EventList.Add(new LogEntry(EventType.Event, user.Name + " is joined", startDate));
        //    }
        //    foreach (Chat chat in Chats)
        //    {
        //        if (chat.Users.Count > 2)
        //        {
        //            foreach (Message message in chat.Messages)
        //            {
        //                EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message in '{chat.Title}' group chat", message.SendTime));
        //            }
        //        }
        //        else
        //        {
        //            foreach (Message message in chat.Messages)
        //            {
        //                EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message to {chat.Users.Find(user => user.Name != message.Sender.Name).Name}", message.SendTime));
        //            }
        //        }
        //    }

        //    using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
        //    {
        //        dataBase.Users.AddRange(Users);
        //        dataBase.Chats.AddRange(Chats);
        //        dataBase.EventList.AddRange(EventList);
        //        dataBase.SaveChanges();
        //    }
        //}

        private void ResetOnlineStatus()
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                foreach (User user in dataBase.Users)
                {
                    user.IsOnline = OnlineStatus.Offline;
                }
                dataBase.SaveChanges();
            }
        }

        public AuthorizationResponse AuthorizeUser(string name)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                User existingUser = dataBase.Users.FirstOrDefault(user => user.Name == name);
                if (existingUser != null)
                {
                    if (existingUser.IsOnline == OnlineStatus.Offline)
                    {
                        return new AuthorizationResponse("Already exists", name, existingUser.UserId);
                    }
                    else
                    {
                        return new AuthorizationResponse("Name is taken");
                    }
                }
                else
                {
                    if (dataBase.Chats.FirstOrDefault(chat => chat.Title == "Public chat") == null)
                    {
                        dataBase.Chats.Add(new Chat("Public chat", new List<User>()));
                        dataBase.SaveChanges();
                    }
                    Chat publicChat = dataBase.Chats.FirstOrDefault(chat => chat.Title == "Public chat");
                    User newUser = new User(name, OnlineStatus.Offline);
                    dataBase.Users.Add(newUser);
                    publicChat?.Users.Add(newUser);
                    dataBase.SaveChanges();

                    return new AuthorizationResponse("New user added", name, newUser.UserId);
                }
            }
        }
        public void SetOnlineStatus(int userId)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                User targetUser = dataBase.Users.FirstOrDefault(user => user.UserId == userId);
                targetUser.IsOnline = OnlineStatus.Online;
                dataBase.EventList.Add(new LogEntry(EventType.Event, $"{targetUser.Name} is logged in", DateTime.Now));
                dataBase.SaveChanges();
                UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(targetUser.Name, targetUser.UserId, targetUser.IsOnline));
            }
        }
        public void SetOfflineStatus(int userId)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                User targetUser = dataBase.Users.FirstOrDefault(user => user.UserId == userId);
                targetUser.IsOnline = OnlineStatus.Offline;
                dataBase.EventList.Add(new LogEntry(EventType.Event, $"{targetUser.Name} is logged out", DateTime.Now));
                dataBase.SaveChanges();
                UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(targetUser.Name, targetUser.UserId, targetUser.IsOnline));
            }
        }

        public GetUserListResponse GetUserList(int userId)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                List<User> userList = dataBase.Users.Include(user => user.Chats).Where(user => user.UserId != userId).ToList();
                return new GetUserListResponse("Success", userList);
            }
        }

        public GetChatListResponse GetChatList(int userId)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                List<Chat> chatList = dataBase.Chats.Include(chat => chat.Users).Include(chat => chat.Messages).Where(chat => chat.Users.FirstOrDefault(user => user.UserId == userId) != null).ToList();
                return new GetChatListResponse("Success", chatList);
            }
        }

        public CreateNewChatResponse CreateNewChat(string title, List<int> userIdList)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                List<User> userList = dataBase.Users.Include(user => user.Chats).ToList();

                Chat newChat = new Chat();
                int userCounter = 0;

                foreach (User user in userList)
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
                    dataBase.Chats.Add(newChat);
                    dataBase.SaveChanges();
                    NewChatCreated?.Invoke(newChat);
                    return new CreateNewChatResponse("Success");
                }
                else
                {
                    return new CreateNewChatResponse("Failure");
                }
            }
        }

        public SendMessageResponse AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        {
            try
            {
                using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
                {
                    User sender = dataBase.Users.FirstOrDefault(user => user.UserId == senderId);

                    if (sender != null)
                    {
                        Chat targetChat = dataBase.Chats.Include(chat => chat.Users).FirstOrDefault(chat => chat.ChatId == chatId);

                        if (targetChat != null)
                        {
                            Message message = new Message(sender, targetChat, text, sendTime);
                            targetChat.Messages.Add(message);

                            if (targetChat.Users.Count > 2)
                            {
                                dataBase.EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message in '{targetChat.Title}' group chat", message.SendTime));
                            }
                            else
                            {
                                dataBase.EventList.Add(new LogEntry(EventType.Message, $"{message.Sender.Name} sent а message to {targetChat.Users.Find(user => user.Name != message.Sender.Name).Name}", message.SendTime));
                            }

                            dataBase.SaveChanges();
                            MessageReceived?.Invoke(message);

                            return new SendMessageResponse("The message has been delivered");
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
            }
            catch
            {
                return new SendMessageResponse("The message was not delivered");
            }
        }

        public GetEventListResponse GetEventLog(DateTime from, DateTime to)
        {
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                List<LogEntry> logResponseList = dataBase.EventList.Where(entry => entry.DateTime >= from && entry.DateTime <= to).ToList();

                return new GetEventListResponse("Success", logResponseList);
            }
        }

        public List<LogEntry> GetEventLog()
        {
            List<LogEntry> eventList = new List<LogEntry>();
            using (DataBaseContext dataBase = new DataBaseContext(_connectionString))
            {
                foreach (LogEntry entry in dataBase.EventList)
                {
                    eventList.Add(entry);
                }
            }
            return eventList;
        }
    }
}
