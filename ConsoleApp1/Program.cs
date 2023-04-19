using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
        static Socket serverSocket;
        static void Main(string[] args)
        {
            WatsonWsServer server = new WatsonWsServer("192.168.137.80", 8090, false);
            //WatsonWsServer server = new WatsonWsServer(new Uri("https://192.168.137.80:23"));
            server.StartAsync();
            server.AcceptInvalidCertificates = true;
            Console.WriteLine($"Server start");
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

                //if (Regex.IsMatch(text, "^GET", RegexOptions.IgnoreCase))
                //{
                //    Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                //    // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                //    // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                //    // 3. Compute SHA-1 and Base64 hash of the new value
                //    // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                //    string swk = Regex.Match(text, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                //    string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                //    byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                //    string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                //    // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                //    byte[] response = Encoding.UTF8.GetBytes(
                //        "HTTP/1.1 101 Switching Protocols\r\n" +
                //        "Connection: Upgrade\r\n" +
                //        "Upgrade: websocket\r\n" +
                //        "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                //    await server.SendAsync(e.Client.Guid, response);
                //}


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

        private static void OnAccept(IAsyncResult result)
        {
            try
            {
                Socket client = null;
                if (serverSocket != null && serverSocket.IsBound)
                {
                    client = serverSocket.EndAccept(result);
                }
                if (client != null)
                {
                    /* Handshaking and managing ClientSocket */
                }
            }
            catch (SocketException exception)
            {

            }
            finally
            {
                if (serverSocket != null && serverSocket.IsBound)
                {
                    serverSocket.BeginAccept(null, 0, OnAccept, null);
                }
            }
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
