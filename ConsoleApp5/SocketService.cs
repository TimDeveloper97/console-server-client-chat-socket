using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class SocketService
    {
        static ClientWebSocket _client = new ClientWebSocket();
        static CancellationTokenSource _cts = new CancellationTokenSource();

        public async void ConnectToServerAsync()
        {
            await _client.ConnectAsync(new Uri("ws://192.168.137.206:8888/ws"), _cts.Token);
            //UpdateClientState();

            Console.WriteLine(_client.State.ToString());
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await ReadMessage();
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task ReadMessage()
        {
            WebSocketReceiveResult result;
            var message = new ArraySegment<byte>();
            do
            {
                result = await _client.ReceiveAsync(message, _cts.Token);
                if (result.MessageType != WebSocketMessageType.Text)
                    break;
                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                string receivedMessage = Encoding.UTF8.GetString(messageBytes);
                Console.WriteLine("Received: {0}", receivedMessage);
            }
            while (!result.EndOfMessage);
        }

        public async Task SendMessageAsync(string message)
        {
            //if (!CanSendMessage(message))
            //    return;

            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);

            await _client.SendAsync(segmnet, WebSocketMessageType.Text, true, _cts.Token);
            //Console.WriteLine(message);
        }

        public WebSocketState State()
        {
            return _client.State;
        }
    }
}
