namespace MessengerServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MessengerServer.DataObjects;
    using MessengerServer.Network.Broadcasts;
    using MessengerServer.Network.Responses;    

    public class ServerStateManager
    {
        #region Fields

        private MessengerDbRepository _repository;

        #endregion //Fields

        #region Properties

        public List<User> Users { get; set; }

        #endregion //Properties

        #region Events

        public event EventHandler<UserStatusChangedBroadcast> UserStatusChanged;
        public event EventHandler<Chat> NewChatCreated;
        public event EventHandler<Message> MessageReceived;

        #endregion //Events

        #region Constructors

        public ServerStateManager()
        {
            _repository = new MessengerDbRepository();
            Users = new List<User>();
        }

        #endregion //Constructors

        #region Methods

        public void Initialize()
        {
            Users = _repository.GetUserList();
        }

        public AuthorizationResponse AuthorizeUser(string name)
        {
            User existingUser = Users.Find(user => user.Name == name);

            if (existingUser != null)
            {
                if (existingUser.IsOnline == UserStatus.Offline)
                {
                    return new AuthorizationResponse("Success", name, existingUser.UserId);
                }
                else
                {
                    _repository.AddLogEntry(new LogEntry(EventType.Error, $"Authorization error: Name '{name}' is taken"));
                    return new AuthorizationResponse("Name is taken");
                }
            }
            else
            {
                User newUser = new User(name, UserStatus.Offline);
                _repository.AddUser(newUser);

                if (newUser != null)
                {
                    Users.Add(newUser);
                    return new AuthorizationResponse("Success", name, newUser.UserId);
                }
            }

            _repository.AddLogEntry(new LogEntry(EventType.Error, $"Authorization error"));
            return new AuthorizationResponse("Authorization error");
        }

        public void ChangeUserStatus(int userId, UserStatus status)
        {
            User targetUser = Users.Find(user => user.UserId == userId);

            if (targetUser != null)
            {
                targetUser.IsOnline = status;
                string state = (status == UserStatus.Online) ? "logged in" : "logged out";
                _repository.AddLogEntry(new LogEntry(EventType.Event, $"{targetUser.Name} is {state}"));
                UserStatusChanged?.Invoke(this, new UserStatusChangedBroadcast(targetUser.Name, targetUser.UserId, status));
            }
            else
            {
                _repository.AddLogEntry(new LogEntry(EventType.Error, $"Status change error: User with id: {userId} is not found"));
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
            List<Chat> chatList = _repository.GetChatList().FindAll(chat => chat.Users.Exists(user => user.UserId == userId)).ToList();

            if (chatList != null)
            {
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

            _repository.AddLogEntry(new LogEntry(EventType.Error, $"'GetChatList' request processing error"));
            return new GetChatListResponse("Failure", chatList);
        }

        public SendMessageResponse AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        {
            Message message = _repository.AddMessage(senderId, chatId, text, sendTime);

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

                _repository.AddLogEntry(entry);
                MessageReceived?.Invoke(this, message);
                return new SendMessageResponse("The message has been delivered");
            }

            _repository.AddLogEntry(new LogEntry(EventType.Error, $"'SendMessageRequest' processing error"));
            return new SendMessageResponse("The message was not delivered");
        }

        public GetEventListResponse GetEventLog(DateTime from, DateTime to)
        {
            List<LogEntry> eventList = _repository.GetEventLog(from, to);

            if (eventList != null)
            {
                return new GetEventListResponse("Sucess", eventList);
            }

            _repository.AddLogEntry(new LogEntry(EventType.Error, $"'GetEventLog' request processing error"));
            return new GetEventListResponse("Failure", eventList);
        }

        public CreateNewChatResponse CreateNewChat(string title, List<int> userIdList)
        {
            Chat newChat = _repository.AddChat(title, userIdList);

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

            _repository.AddLogEntry(new LogEntry(EventType.Error, $"'CreateNewChat' request processing error"));
            return new CreateNewChatResponse("An error occurred while creating the chat");
        }

        #endregion //Methods
    }
}
