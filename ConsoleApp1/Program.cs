using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WatsonWebsocket;

namespace Server
{
    class Program
    {
        static Dictionary<string, string> _guids = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            WatsonWsServer wsServer = new WatsonWsServer("127.0.0.1", 8888);
            wsServer.StartAsync();

            wsServer.ClientConnected += WsServer_ClientConnected;
            wsServer.ClientDisconnected += WsServer_ClientDisconnected;
            wsServer.MessageReceived += (s,e) =>
            {
                var text = System.Text.Encoding.Default.GetString(e.Data);
                var res = new Response(text);
                
                // set name
                if (!_guids.Any(x => x.Key == res.Sender))
                {
                    _guids.Add(res.Sender, e.Client.Guid.ToString());
                }

                var exist = _guids.Where(x => x.Key == res.Receiver).FirstOrDefault().Value;
                if(exist != null)
                {
                    wsServer.SendAsync(new Guid(exist), text);
                }    

                Console.WriteLine($"{res.Sender}: " + res.Message);
            };

            Process.GetCurrentProcess().WaitForExit();
        }

        private static void WsServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var res = System.Text.Encoding.Default.GetString(e.Data);
            var x = e.Client.Name;
            Console.WriteLine("client: " + res);
        }

        class Response
        {
            public string Sender { get; set; }
            public string Receiver { get; set; }
            public string Message { get; set; }

            public Response(string obj)
            {
                Sender = obj.Substring(1, 7);
                Receiver = obj.Substring(10, 7);
                Message = obj.Substring(18);
            }
        }

        private static void WsServer_ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            Console.WriteLine("client: " + e.Client.Guid.ToString() + " disconnected");
        }

        private static void WsServer_ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("client: " + e.Client.Guid.ToString() + " connected");
        }
    }
}
