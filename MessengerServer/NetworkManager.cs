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
        private const int WS_PORT = 7890;
        private readonly IPAddress _ipAddress = IPAddress.Any;
        private readonly WsServer _wsServer;
        private ServerStateManager _serverState;

        public NetworkManager()
        {
            _serverState = new ServerStateManager();
            _wsServer = new WsServer(_serverState, new IPEndPoint(_ipAddress, WS_PORT));
            _wsServer.UserStatusChanged += HandleUserStatusChanged;
            _wsServer.MessageReceived += HandleMessageReceived;
        }

        public void Start()
        {
            Console.WriteLine($"WebSocketServer: {_ipAddress}:{WS_PORT}");
            _wsServer.Start();
        }

        public void Stop()
        {
            _wsServer.Stop();
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = $"Клиент '{e.ClientName}' отправил сообщение '{e.Message}'.";

            Console.WriteLine(message);

            //_wsServer.Send(message);

        }

        private void HandleUserStatusChanged(UserStatusChangedBroadcast broadcast)
        {
            string state = broadcast.Status == OnlineStatus.Online ? "подключен" : "отключен";
            Console.WriteLine($"Клиент '{broadcast.Name}' {state}.");
        }
    }
}
