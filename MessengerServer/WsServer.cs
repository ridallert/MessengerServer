using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.Broadcasts;
using MessengerServer.Network.EventArgs;
using MessengerServer.Network.Requests;
using MessengerServer.Network.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace MessengerServer
{
    class WsServer
    {
        private readonly IPEndPoint _listenAddress;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;
        private ServerStateManager _serverState;
        private WebSocketServer _server;

        public event Action<UserStatusChangedBroadcast> UserStatusChanged;

        public WsServer(ServerStateManager serverState, IPEndPoint listenAddress)
        {
            _serverState = serverState;
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>(); 
        }

        public void Start(int timeout)
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService("/", () => new WsConnection(this, timeout));
            _server.Start();

            UserStatusChanged += SendUserStatusChangedBroadcast;
            _serverState.NewChatCreated += SendNewChatCreatedReponse;
            _serverState.MessageReceived += SendMessage;
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
                    AuthorizationResponse authorizationResponse = _serverState.AuthorizeUser(authorizationRequest.Name);
                    connection.Send(authorizationResponse.GetContainer());

                    if (authorizationResponse.Result != "NameIsTaken")
                    {
                        connection.Login = authorizationResponse.Name;
                        connection.UserId = authorizationResponse.UserId;
                        UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, connection.UserId.Value, OnlineStatus.Online));
                    }
                    break;

                case nameof(GetUserListRequest):

                    GetUserListRequest getContactsRequest = JsonConvert.DeserializeObject<GetUserListRequest>(container.Payload.ToString());
                    GetUserListResponse getContactsResponse = _serverState.GetContacts(getContactsRequest.Name);
                    connection.Send(getContactsResponse.GetContainer());
                    break;

                case nameof(GetChatListRequest):

                    GetChatListRequest getChatListRequest = JsonConvert.DeserializeObject<GetChatListRequest>(container.Payload.ToString());
                    GetChatListResponse getChatListResponse = _serverState.GetChatList(getChatListRequest.Name);
                    connection.Send(getChatListResponse.GetContainer());
                    break;

                case nameof(CreateNewChatRequest):

                    CreateNewChatRequest createNewChatRequest = JsonConvert.DeserializeObject<CreateNewChatRequest>(container.Payload.ToString());
                    CreateNewChatResponse createNewChatResponse = _serverState.CreateNewChat(createNewChatRequest.Title, createNewChatRequest.UserIdList);
                    connection.Send(createNewChatResponse.GetContainer());
                    break;

                case nameof(SendMessageRequest):

                    SendMessageRequest messageRequest = JsonConvert.DeserializeObject<SendMessageRequest>(container.Payload.ToString());
                    _serverState.AddMessage(messageRequest.SenderId, messageRequest.ChatId, messageRequest.Text, messageRequest.SendTime);
                    var privateMessageResponse = new SendMessageResponse("Success");
                    connection.Send(privateMessageResponse.GetContainer());
                    break;

                case nameof(GetEventListRequest):

                    GetEventListRequest getEventListRequest = JsonConvert.DeserializeObject<GetEventListRequest>(container.Payload.ToString());
                    GetEventListResponse getEventListResponse = _serverState.GetEventLog(getEventListRequest.From, getEventListRequest.To);
                    connection.Send(getEventListResponse.GetContainer());
                    break;
            }
        }
        internal void SendUserStatusChangedBroadcast(UserStatusChangedBroadcast broadcast)
        {
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
            if (_connections.TryRemove(connectionId, out WsConnection connection) && !string.IsNullOrEmpty(connection.Login))
            {
                _serverState.SetUserOffline(connection.Login);
                UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, connection.UserId.Value, OnlineStatus.Offline));
            }
        }
    }
}
