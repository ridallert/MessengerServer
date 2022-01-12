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
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public WsServer(ServerStateManager serverState, IPEndPoint listenAddress)
        {
            _serverState = serverState;
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
            UserStatusChanged += SendUserStatusChangedBroadcast;
        }

        public void Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService("/", () => new WsConnection(this));
            //_server.AddWebSocketService<WsConnection>("/", AddService);
            _server.Start();
            _serverState.PrivateMessageReceived += SendPrivateMessage;
        }

        //WsConnection AddService()
        //{
        //    WsConnection client = new WsConnection(this);

        //    //client.AddServer(this);
        //    return client;
        //}

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

        //public void Send(string message)
        //{
        //    var messageBroadcast = new MessageBroadcast(message).GetContainer();

        //    foreach (var connection in _connections)
        //    {
        //        connection.Value.Send(messageBroadcast);
        //    }
        //}

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
                        connection.Login = authorizationRequest.Name;
                        UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, OnlineStatus.Online));
                    }

                    break;

                case nameof(GetContactsRequest):

                    GetContactsRequest getContactsRequest = JsonConvert.DeserializeObject<GetContactsRequest>(container.Payload.ToString());
                    GetContactsResponse getContactsResponse = _serverState.GetContacts(getContactsRequest.Name);
                    connection.Send(getContactsResponse.GetContainer());
                    break;  

                case nameof(SendPrivateMessageRequest):

                    var privateMessageRequest = JsonConvert.DeserializeObject<SendPrivateMessageRequest>(container.Payload.ToString());
                    _serverState.AddPrivateMessage(privateMessageRequest.Sender, privateMessageRequest.Receiver, privateMessageRequest.Text, privateMessageRequest.SendTime);
                    var privateMessageResponse = new SendMessageResponse("Success");
                    break;
                case nameof(GetPrivateMessageListRequest):

                    GetPrivateMessageListRequest getPrivateMessageListRequest = JsonConvert.DeserializeObject<GetPrivateMessageListRequest>(container.Payload.ToString());
                    GetPrivateMessageListResponse getPrivateMessageListResponse = _serverState.GetPrivateMessageList(getPrivateMessageListRequest.Name);
                    connection.Send(getPrivateMessageListResponse.GetContainer());
                    break;

                case nameof(GetPublicMessageListRequest):

                    GetPublicMessageListRequest getPublicMessageListRequest = JsonConvert.DeserializeObject<GetPublicMessageListRequest>(container.Payload.ToString());
                    //GetPublicMessageListResponse getPublicMessageListResponse = _serverState.GetPublicMessageList(getPublicMessageListRequest.Name);
                    //connection.Send(getPublicMessageListResponse.GetContainer());
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
        internal void SendPrivateMessage(object sender, Message message)
        {
            foreach (var connection in _connections)
            {
                if (connection.Value.Login == message.Sender || connection.Value.Login == message.Receiver)
                {
                    var privateMessageResponse = new PrivateMessageReceivedResponse(message.Sender, message.Receiver, message.Text, message.SendTime);
                    connection.Value.Send(privateMessageResponse.GetContainer());
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
                UserStatusChanged?.Invoke(new UserStatusChangedBroadcast(connection.Login, OnlineStatus.Offline));
            }
        }

    }
}
