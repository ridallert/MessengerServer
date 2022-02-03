using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.Broadcasts;
using MessengerServer.Network.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MessengerServer
{
    class NetworkManager
    {
        private const int _port = 7890;
        private readonly IPAddress _ipAddress = IPAddress.Any;
        private readonly WsServer _wsServer;
        private ServerStateManager _serverState;

        public NetworkManager()
        {
            _serverState = new ServerStateManager();
            _wsServer = new WsServer(_serverState, new IPEndPoint(_ipAddress, _port));
            _wsServer.UserStatusChanged += HandleUserStatusChanged;
            _serverState.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {_ipAddress}:{_port}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        private void OnMessageReceived(Message message)
        {
            string receiver;
            if (message.Chat.Users.Count == 2)
            {
                receiver = message.Chat.Users.Find(user => user.Name != message.Sender.Name).Name;
            }
            else
            {
                receiver = message.Chat.Title;
            }
            Console.WriteLine($"Клиент '{message.Sender.Name}' отправил сообщение '{message.Text}' для '{receiver}'");
        }

        private void HandleUserStatusChanged(UserStatusChangedBroadcast broadcast)
        {
            string state = broadcast.Status == OnlineStatus.Online ? "подключен" : "отключен";
            Console.WriteLine($"Клиент '{broadcast.Name}' {state}");
        }
    }
}
