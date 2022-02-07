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
        private readonly int _timeout;          //запихнуть в конструктов WsServer
        private readonly int _port;             //запихнуть в конструктов WsServer
        private readonly IPAddress _ipAddress;  //запихнуть в конструктов WsServer

        private readonly WsServer _wsServer;
        //private readonly ServerStateManager _serverState;

        public NetworkManager()
        {
            

            _wsServer = new WsServer(); //запихнуть в конструктов WsServer
           
        }

        //public void Start()
        //{
        //    Console.WriteLine($"WebSocketServer: {_ipAddress}:{_port}");
        //    _wsServer.Start();
        //}

        public void Stop()
        {
            _wsServer.Stop();
        }
    }
}
