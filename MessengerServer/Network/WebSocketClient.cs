﻿using Messenger.Common;
using Messenger.Network.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Messenger.Network.Requests;

namespace Messenger.Network
{
    public class WebSocketClient
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private int _sending;
        private string _login;

        private WebSocket _socket;
        public bool IsConnected
        {
            get
            {
                return _socket?.ReadyState == WebSocketState.Open;
            }
        }

        public event EventHandler<ConnectionStateChangedEventArgs> Connected;
        public event EventHandler<ConnectionStateChangedEventArgs> Disconnected;
        public event EventHandler MessageReceived;

        private void Send()
        {
            if (!IsConnected)
                return;
            if (!_sendQueue.TryDequeue(out var message) && Interlocked.CompareExchange(ref _sending, 0, 1) == 1)
                return;
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }
        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                Disconnected?.Invoke(this, new ConnectionStateChangedEventArgs(_login));
                return;
            }
            Send();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            //if (e.IsText == false)
            //    return;

            //MessageContainer container = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            //switch (container.Identifier)
            //{
            //   // case nameof()
            //}



            Console.WriteLine(e.Data);
            Console.WriteLine("Enter message:");
        }
        public void SetParams(string ipAddress, int port)
        {
            _socket = new WebSocket($"ws://{ipAddress}:{port}/test");

        }
        public void Connect()
        {
            _socket.OnOpen += OnOpen;
            _socket.OnMessage += OnMessage;
            _socket?.ConnectAsync();
        }
        public void Disconnect()
        {
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket?.CloseAsync();
        }
        public void Login(string login)
        {
            _login = login;
            _sendQueue.Enqueue(new AuthorizationRequest(_login).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }

        public void SendPrivateMessage(User sender, User receiver, string message, DateTime sendTime)
        {

            _socket.Send($"from {sender.Name} to {receiver.Name} ({sendTime}): \n\t\t{message}\n");
        }

        
        private void OnOpen(object sender, System.EventArgs e)
        {
            Connected?.Invoke(this, new ConnectionStateChangedEventArgs(_login));
        }
        private void OnClose(object sender, CloseEventArgs e)
        {
            Disconnected?.Invoke(this, new ConnectionStateChangedEventArgs(_login));
        }
    }
}
