﻿namespace MessengerServer
{
    using MessengerServer.Common;
    using MessengerServer.Configurations;
    using MessengerServer.Network;
    using MessengerServer.Network.Broadcasts;
    using MessengerServer.Network.Requests;
    using MessengerServer.Network.Responses;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using WebSocketSharp.Server;

    class WsServer
    {
        private readonly ConfigManager _configs;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;
        private readonly DataBaseManager _dataBaseManager;
        private WebSocketServer _server;

        public event Action<UserStatusChangedBroadcast> UserStatusChanged;

        public WsServer()
        {
            _configs = new ConfigManager();
            _dataBaseManager = new DataBaseManager();
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
        }

        public void Start()
        {
            _server = new WebSocketServer(_configs.IpAddress, _configs.Port, false);
            _server.AddWebSocketService("/", () => new WsConnection(this, _configs.Timeout));
            _server.Start();

            Console.WriteLine($"WebSocketServer: {_configs.IpAddress}:{_configs.Port}");

            UserStatusChanged += SendUserStatusChangedBroadcast;
    
            _dataBaseManager.MessageReceived += SendMessage;
            _dataBaseManager.NewChatCreated += SendNewChatCreatedReponse;
        }

        public void Stop()
        {
            _server?.Stop();
            _server = null;

            var connections = _connections.Select(item => item.Value).ToArray();
            foreach (var connection in connections)
            {
                connection.Close();
            }

            _connections.Clear();
        }

        internal void HandleRequest(Guid clientId, MessageContainer container)
        {
            if (!_connections.TryGetValue(clientId, out WsConnection connection))
                return;

            switch (container.Identifier)
            {
                case nameof(AuthorizationRequest):

                    AuthorizationRequest authorizationRequest = JsonConvert.DeserializeObject<AuthorizationRequest>(container.Payload.ToString());
                    AuthorizationResponse authorizationResponse = _dataBaseManager.AuthorizeUser(authorizationRequest.Name);
                    connection.Send(authorizationResponse.GetContainer());

                    if (authorizationResponse.Result != "Name is taken")
                    {
                        connection.Login = authorizationResponse.Name;
                        connection.UserId = authorizationResponse.UserId;
                        UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, connection.UserId.Value, OnlineStatus.Online));
                    }
                    break;

                case nameof(GetUserListRequest):

                    GetUserListRequest getUserListRequest = JsonConvert.DeserializeObject<GetUserListRequest>(container.Payload.ToString());
                    GetUserListResponse getUserListResponse = _dataBaseManager.GetUserList(getUserListRequest.UserId);
                    connection.Send(getUserListResponse.GetContainer());
                    break;

                case nameof(GetChatListRequest):

                    GetChatListRequest getChatListRequest = JsonConvert.DeserializeObject<GetChatListRequest>(container.Payload.ToString());
                    GetChatListResponse getChatListResponse = _dataBaseManager.GetChatList(getChatListRequest.UserId);
                    connection.Send(getChatListResponse.GetContainer());
                    break;

                case nameof(CreateNewChatRequest):

                    CreateNewChatRequest createNewChatRequest = JsonConvert.DeserializeObject<CreateNewChatRequest>(container.Payload.ToString());
                    CreateNewChatResponse createNewChatResponse = _dataBaseManager.CreateNewChat(createNewChatRequest.Title, createNewChatRequest.UserIdList);
                    connection.Send(createNewChatResponse.GetContainer());
                    break;

                case nameof(SendMessageRequest):

                    SendMessageRequest messageRequest = JsonConvert.DeserializeObject<SendMessageRequest>(container.Payload.ToString());
                    _dataBaseManager.AddMessage(messageRequest.SenderId, messageRequest.ChatId, messageRequest.Text, messageRequest.SendTime);
                    var privateMessageResponse = new SendMessageResponse("Success");
                    connection.Send(privateMessageResponse.GetContainer());
                    break;

                case nameof(GetEventListRequest):

                    GetEventListRequest getEventListRequest = JsonConvert.DeserializeObject<GetEventListRequest>(container.Payload.ToString());
                    GetEventListResponse getEventListResponse = _dataBaseManager.GetEventLog(getEventListRequest.From, getEventListRequest.To);
                    connection.Send(getEventListResponse.GetContainer());
                    break;
            }
        }
        internal void SendUserStatusChangedBroadcast(UserStatusChangedBroadcast broadcast)
        {
            string state = broadcast.Status == OnlineStatus.Online ? "connected" : "disconnected";
            Console.WriteLine($"Client '{broadcast.Name}' is {state}");

            foreach (var connection in _connections)
            {
                if (connection.Value.Login != broadcast.Name)
                {
                    connection.Value.Send(broadcast.GetContainer());
                }
            }
        }

        internal void SendNewChatCreatedReponse(Chat chat)
        {
            foreach (var connection in _connections)
            {
                foreach (User user in chat.Users)
                {
                    if (connection.Value.Login == user.Name)
                    {
                        NewChatCreatedResponse newChatCreatedResponse = new NewChatCreatedResponse(chat);
                        connection.Value.Send(newChatCreatedResponse.GetContainer());
                    }
                }
            }
        }

        internal void SendMessage(Message message)
        {
            if (message.Chat.Users.Count == 2)
            {
                string receiver = message.Chat.Users.Find(user => user.Name != message.Sender.Name).Name;
                Console.WriteLine($"Client '{message.Sender.Name}' sent message '{message.Text}' for '{receiver}'");
            }
            else
            {
                string receiver = message.Chat.Title;
                Console.WriteLine($"Client '{message.Sender.Name}' sent message '{message.Text}' in '{receiver}' chat");
            }

            foreach (var connection in _connections)
            {
                if (message.Chat.Users.Exists(user => user.UserId == connection.Value.UserId))
                {
                    var messageResponse = new MessageReceivedResponse(message.MessageId, message.Sender.UserId, message.Chat.ChatId,message.Sender.Name, message.Text, message.SendTime);
                    connection.Value.Send(messageResponse.GetContainer());
                }
            }
        }

        internal void AddConnection(WsConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
        }

        internal void FreeConnection(Guid connectionId)
        {
            if (_connections.TryRemove(connectionId, out WsConnection connection) && connection.UserId != null)
            {
                _dataBaseManager.SetOfflineStatus(connection.UserId.Value);
                UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, connection.UserId.Value, OnlineStatus.Offline));
            }
        }
    }
}
