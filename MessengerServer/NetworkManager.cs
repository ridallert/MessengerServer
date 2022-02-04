using MessengerServer.Common;
using MessengerServer.Configurations;
using MessengerServer.Network;
using MessengerServer.Network.Broadcasts;
using MessengerServer.Network.EventArgs;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly int _timeout;
        private readonly int _port;
        private readonly IPAddress _ipAddress;

        private readonly WsServer _wsServer;
        private readonly ServerStateManager _serverState;

        public NetworkManager()
        {
            var configManager = new ConfigManager();

            _ipAddress = configManager.IpAddress;
            _port = configManager.Port;
            _timeout = configManager.Timeout;

            ConnectionStringSettings connectionSettings = configManager.ConnectionSettings;
            DataBaseManager dataBaseManager;

            try
            {
                dataBaseManager = new DataBaseManager(connectionSettings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Data Base Connection Error:\n" + e.Message);
                dataBaseManager = new DataBaseManager(configManager.GetDefaultConnectionString());
            }


            _serverState = new ServerStateManager();
            _wsServer = new WsServer(_serverState, new IPEndPoint(_ipAddress, _port));
            _wsServer.UserStatusChanged += HandleUserStatusChanged;
            _serverState.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {_ipAddress}:{_port}");
            _wsServer.Start(_timeout);
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
            Console.WriteLine($"Client '{message.Sender.Name}' sent message '{message.Text}' for '{receiver}'");
        }

        private void HandleUserStatusChanged(UserStatusChangedBroadcast broadcast)
        {
            string state = broadcast.Status == OnlineStatus.Online ? "connected" : "disconnected";
            Console.WriteLine($"Client '{broadcast.Name}' is {state}");
        }
    }
}
