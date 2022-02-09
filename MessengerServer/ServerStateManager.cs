using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.Broadcasts;
using MessengerServer.Network.EventArgs;
using MessengerServer.Configurations;
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
        private DbContextManager _dataBaseManager;
        public List<User> Users { get; set; }

        public event EventHandler<UserStatusChangedBroadcast> UserStatusChanged;
        public event EventHandler<Chat> NewChatCreated;
        public event EventHandler<Message> MessageReceived;
        public ServerStateManager()
        {
            _dataBaseManager = new DbContextManager();
        }
        public void Initialize()
        {
            Users = _dataBaseManager.GetUserList();
        }
        public AuthorizationResponse AuthorizeUser(string name)
        {
            User existingUser = Users.Find(user => user.Name == name);

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
                User newUser = new User(name, OnlineStatus.Offline);
                _dataBaseManager.AddUser(newUser);

                if (newUser != null)
                {
                    Users.Add(newUser);
                    return new AuthorizationResponse("New user added", name, newUser.UserId);
                }
            }

            return new AuthorizationResponse("Error");
        }

        public void ChangeUserStatus(int userId, OnlineStatus status)
        {
            User targetUser = Users.Find(user => user.UserId == userId);

            if (targetUser != null)
            {
                targetUser.IsOnline = status;

                string state = (status == OnlineStatus.Online) ? "logged in" : "logged out";
                _dataBaseManager.AddLogEntry(new LogEntry(EventType.Event, $"{targetUser.Name} is {state}"));

                UserStatusChanged?.Invoke(this, new UserStatusChangedBroadcast(targetUser.Name, targetUser.UserId, status));
            }
            else
            {
                throw new Exception($"User with id: {userId} does not exist");
            }
        }

        public GetUserListResponse GetPersonalUserList(int userId)
        {
            List<User> userList = Users.FindAll(user => user.UserId != userId).ToList();

            return new GetUserListResponse("Success", userList);
        }

        public GetChatListResponse GetChatList(int userId)
        {
            List<Chat> chatList = _dataBaseManager.GetChatList().FindAll(chat => chat.Users.Exists(user => user.UserId == userId)).ToList();

            foreach (Chat chat in chatList)
            {
                foreach (User chatUser in chat.Users)
                {
                    User statusHolder = Users.Find(user => user.UserId == chatUser.UserId);
                    if (statusHolder != null)
                    {
                        chatUser.IsOnline = statusHolder.IsOnline;
                    }
                }
            }
            return new GetChatListResponse("Success", chatList);
        }

        public SendMessageResponse AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        {
            Message message = _dataBaseManager.AddMessage(senderId, chatId, text, sendTime);

            if (message != null)
            {
                LogEntry entry;
                if (message.Chat.Users.Count > 2 || message.Chat.Title == "Public chat")
                {
                    entry = new LogEntry(EventType.Message, $"{message.Sender.Name} sent а private message in '{message.Chat.Title}' group chat", message.SendTime);
                }
                else
                {
                    entry = new LogEntry(EventType.Message, $"{message.Sender.Name} sent а message to {message.Chat.Users.Find(user => user.Name != message.Sender.Name).Name}", message.SendTime);
                }

                _dataBaseManager.AddLogEntry(entry);
                MessageReceived?.Invoke(this, message);

                return new SendMessageResponse("The message has been delivered");
            }

            return new SendMessageResponse("The message was not delivered");
        }

        public GetEventListResponse GetEventLog(DateTime from, DateTime to)
        {
            List<LogEntry> eventList = _dataBaseManager.GetEventLog(from, to);

            return new GetEventListResponse("Sucess", eventList);
        }

        public CreateNewChatResponse CreateNewChat(string title, List<int> userIdList)
        {
            Chat newChat = _dataBaseManager.AddChat(title, userIdList);
            if (newChat != null)
            {
                foreach (User chatUser in newChat.Users)
                {
                    User statusHolder = Users.Find(user => user.UserId == chatUser.UserId);
                    if (statusHolder != null)
                    {
                        chatUser.IsOnline = statusHolder.IsOnline;
                    }
                }
                NewChatCreated?.Invoke(this, newChat);
                return new CreateNewChatResponse("Chat created");
            }
            return new CreateNewChatResponse("An error occurred while creating the chat");
        }
    }
}
