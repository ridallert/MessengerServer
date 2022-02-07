namespace MessengerServer
{
    using MessengerServer.Network;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using WebSocketSharp;
    using WebSocketSharp.Server;
    using System.Timers;

    class WsConnection : WebSocketBehavior
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WsServer _server;
        private int _sending;
        private System.Timers.Timer _timer;
        public Guid Id { get; }
        public string Login { get; set; }
        public int? UserId { get; set; }

        public bool IsConnected => Context.WebSocket?.ReadyState == WebSocketState.Open;

        public WsConnection(WsServer server, int timeout)
        {            
            _server = server;
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _sending = 0;

            _timer = new System.Timers.Timer { AutoReset = false, Interval = timeout * 1000 };
            _timer.Elapsed += OnTimerElapsed;

            Id = Guid.NewGuid();
            _timer.Start();
        }
        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Close();
            Console.WriteLine($"Client '{Login}' is disabled due to inactivity");
        }

        public void Send(MessageContainer container)
        {
            if (!IsConnected)
                return;

            _sendQueue.Enqueue(container);
            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                SendImpl();
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        protected override void OnOpen()
        {
            _server.AddConnection(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _server.FreeConnection(Id);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            ResetTimer();

            if (e.IsText)
            {
                var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);
                _server.HandleRequest(Id, message);
            }
        }

        private void SendCompleted(bool completed)
        {            
            if (!completed)
            {
                Console.WriteLine("An error occurred while sending the response");
                _server.FreeConnection(Id);
                Context.WebSocket.CloseAsync();
                return;
            }

            SendImpl();
        }

        private void SendImpl()
        {
            if (!IsConnected)
                return;

            if (!_sendQueue.TryDequeue(out var message) && Interlocked.CompareExchange(ref _sending, 0, 1) == 1)
                return;

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessage = JsonConvert.SerializeObject(message, settings);
            SendAsync(serializedMessage, SendCompleted);
        }
    }
}
