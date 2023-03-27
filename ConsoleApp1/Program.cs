using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WatsonWebsocket;

namespace Server
{
    class Program
    {
        /// <summary>
        /// chứa tất cả GUID theo name
        /// </summary>
        static Dictionary<string, string> _guids = new Dictionary<string, string>();
        /// <summary>
        /// Chứa tất cả tin nhắn cũ và mới với key là gộp 2 name
        /// </summary>
        static Dictionary<string, List<Response>> _messages = new Dictionary<string, List<Response>>();

        static void Main(string[] args)
        {
            WatsonWsServer server = new WatsonWsServer("127.0.0.1", 8888);
            server.StartAsync();

            server.ClientConnected += (s, e) =>
            {
                Console.WriteLine("==================================================================");
                Console.WriteLine("client: " + e.Client.Guid.ToString() + " connected");
                Console.WriteLine("==================================================================");
            };
            server.ClientDisconnected += WsServer_ClientDisconnected;
            server.MessageReceived += async (s, e) =>
            {
                var text = System.Text.Encoding.Default.GetString(e.Data);
                var res = new Response(text);

                // set name
                if (!_guids.Any(x => x.Key == res.Sender))
                {
                    _guids.Add(res.Sender, e.Client.Guid.ToString());
                }

                if (res.Receiver == null)
                {
                    var existOldMessage = _messages.Where(x => x.Key.Contains(res.Sender)).FirstOrDefault().Value;
                    if (existOldMessage != null)
                    {
                        foreach (var item in existOldMessage)
                        {
                            //gửi lại cho chính nó tất cả nội dung tin nhắn cũ
                            await server.SendAsync(e.Client.Guid, item.Message);
                        }
                    }

                    return;
                }

                var graft = GraftName(res.Sender, res.Receiver);

                //nhắn tin trực tiếp cho Receiver sau khi đã connected
                var existGuid = _guids.Where(x => x.Key == res.Receiver).FirstOrDefault().Value;
                if (existGuid != null)
                {
                    await server.SendAsync(new Guid(existGuid), text);

                    //add to one - one
                    var existOldMessage = _messages.Where(x => x.Key == graft).FirstOrDefault().Value;
                    if (existOldMessage == null)
                    {
                        _messages.Add(graft, new List<Response> { res });
                    }
                    else
                    {
                        existOldMessage.Add(res);
                    }
                }
                else
                {
                    //save vào ram khi 1 trong 2 offline
                    var existOldMessage = _messages.Where(x => x.Key == graft).FirstOrDefault().Value;
                    if (existOldMessage == null)
                    {
                        _messages.Add(graft, new List<Response> { res });
                    }
                    else
                    {
                        existOldMessage.Add(res);
                    }
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
                if (obj.Count() > 18)
                {
                    Receiver = obj.Substring(10, 7);
                    Message = obj.Substring(18);
                }
            }
        }

        static string GraftName(string name1, string name2)
        {
            if (name1.CompareTo(name2) < 0)
                return name1 + name2;
            else if (name1.CompareTo(name2) > 0)
                return name2 + name1;

            return null;
        }

        private static void WsServer_ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            Console.WriteLine("==================================================================");
            Console.WriteLine("client: " + e.Client.Guid.ToString() + " disconnected");
            Console.WriteLine("==================================================================");
        }

        private static void WsServer_ClientConnected(object sender, ConnectionEventArgs e)
        {
        }
    }
}
