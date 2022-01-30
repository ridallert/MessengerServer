namespace MessengerServer
{
    using MessengerServer.Common;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    public class DataBaseManager
    {
        private DataBaseContext _dataBase;

        public void Connect(string connectionString)
        {
            try
            {
                _dataBase = new DataBaseContext(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
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
        public List<Chat> GetGroupChats()
        {
            List<Chat> chatList = new List<Chat>();

            foreach (Chat chat in _dataBase.Chats.Include(chat => chat.Users))
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
