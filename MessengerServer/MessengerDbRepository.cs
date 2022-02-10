namespace MessengerServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using MessengerServer.Configurations;
    using MessengerServer.DataObjects;
    using MessengerServer.Data;
    
    public class MessengerDbRepository
    {
        #region Fields

        private string _connectionString;

        #endregion //Fields

        #region Constructors

        public MessengerDbRepository()
        {
            var configManager = new ConfigManager();
            _connectionString = configManager.ConnectionSettings.ToString();

            try
            {
                MessengerDbContext _dataBase = new MessengerDbContext(_connectionString);
                _dataBase?.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Database connection error:\n" + e.Message);
                _connectionString = configManager.GetDefaultConnectionString().ToString();

                MessengerDbContext _dataBase = new MessengerDbContext(_connectionString);
                _dataBase?.Dispose();
            }
        }

        #endregion //Constructors

        #region Methods

        public bool AddUser(User newUser)
        {
            try
            {
                using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
                {
                    if (dataBase.Chats.FirstOrDefault(chat => chat.Title == "Public chat") == null)
                    {
                        dataBase.Chats.Add(new Chat("Public chat", new List<User>()));
                        dataBase.SaveChanges();
                    }
                    Chat publicChat = dataBase.Chats.FirstOrDefault(chat => chat.Title == "Public chat");
                    dataBase.Users.Add(newUser);
                    publicChat.Users.Add(newUser);
                    dataBase.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public Chat AddChat(string title, List<int> userIdList)
        {
            using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
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
                    newChat.Title = title;
                    dataBase.Chats.Add(newChat);
                    dataBase.SaveChanges();
                    return newChat;
                }
                else
                {
                    return null;
                }
            }
        }

        public Message AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        {
            try
            {
                using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
                {
                    User sender = dataBase.Users.FirstOrDefault(user => user.UserId == senderId);

                    if (sender != null)
                    {
                        Chat targetChat = dataBase.Chats.Include(chat => chat.Users).FirstOrDefault(chat => chat.ChatId == chatId);

                        if (targetChat != null)
                        {
                            Message message = new Message(sender, targetChat, text, sendTime);
                            targetChat.Messages.Add(message);
                            dataBase.SaveChanges();

                            return message;
                        }
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool AddLogEntry(LogEntry entry)
        {
            try
            {
                using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
                {
                    dataBase.EventList.Add(entry);
                    dataBase.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<User> GetUserList()
        {
            using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
            {
                return dataBase.Users.ToList();
            }
        }

        public List<Chat> GetChatList()
        {
            try
            {
                using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
                {
                    return dataBase.Chats.Include(chat => chat.Users).Include(chat => chat.Messages).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<LogEntry> GetEventLog(DateTime from, DateTime to)
        {
            using (MessengerDbContext dataBase = new MessengerDbContext(_connectionString))
            {
                return dataBase.EventList.Where(entry => entry.DateTime >= from && entry.DateTime <= to).ToList();
            }
        }

        #endregion //Methods
    }
}
