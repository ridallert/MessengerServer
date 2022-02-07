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

        private DataBaseManager _dataBaseManager;

        public List<User> Users { get; set; }
        public List<Chat> Chats { get; set; }
        private List<LogEntry> EventList { get; set; }

        //public event Action<UserStatusChangedBroadcast> UserStatusChanged;
        
        public event Action<Message> MessageReceived;


        public ServerStateManager()
        {
            //ConfigManager configManager = new ConfigManager();
            _dataBaseManager = new DataBaseManager();
            //_dataBaseManager.InitializeDataBase();

            
            EventList = _dataBaseManager.GetEventLog();
        }

        //public AuthorizationResponse AuthorizeUser(string name)
        //{     
        //    return _dataBaseManager.AuthorizeUser(name);
        //}
        //public GetUserListResponse GetUserList(int userId)
        //{
        //    return _dataBaseManager.GetUserList(userId);
        //}
        //public GetChatListResponse GetChatList(int userId)
        //{
        //    return _dataBaseManager.GetChatList(userId);
        //}
        //public CreateNewChatResponse CreateNewChat(string title, List<int> userIdList)
        //{
        //    return _dataBaseManager.CreateNewChat(title, userIdList);
        //}

        

        
        
        //public void AddMessage(int senderId, int chatId, string text, DateTime sendTime)
        //{
        //    User sender = Users.Find(user => user.UserId == senderId);
        //    if (sender != null)
        //    {

        //        Chat targetChat = Chats.Find(chat => chat.ChatId == chatId);
        //        if (targetChat != null)
        //        {
        //            Message message = new Message(sender, targetChat, text, sendTime);
        //            targetChat.Messages.Add(message);
        //            MessageReceived?.Invoke(message);
        //        }
        //        else
        //        {
        //            throw new Exception($"AddMessage: Chat with id = {chatId} does not exist");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception($"AddMessage: Sender with id = {sender} does not exist");
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

        
    }
}
