using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WatsonWebsocket;

namespace Client1
{
    class Program
    {
        const string name2 = "[client2]";
        const string name1 = "[client1]";

        [Obsolete]
        static async Task Main(string[] args)
        {
            //try
            //{
            //            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("192.168.137.80");
            //            IPAddress ipAddress = ipHostInfo.AddressList[0];
            //            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 80);

            //            Socket client = new Socket(
            //ipEndPoint.AddressFamily,
            //SocketType.Stream,
            //ProtocolType.Tcp);

            //            await client.ConnectAsync(ipEndPoint);
            //            while (true)
            //            {
            //                // Send message.
            //                var message = "Hi friends EOM";
            //                var messageBytes = Encoding.UTF8.GetBytes(message);
            //                _ = await client.SendAsync(messageBytes, SocketFlags.None);
            //                Console.WriteLine($"Socket client sent message: \"{message}\"");
            //                client.DisconnectAsync(new SocketAsyncEventArgs());
            //                // Receive ack.
            //                var buffer = new byte[1_024];
            //                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            //                var response = Encoding.UTF8.GetString(buffer, 0, received);
            //                if (response == "ACK")
            //                {
            //                    Console.WriteLine(
            //                        $"Socket client received acknowledgment: \"{response}\"");
            //                    break;
            //                }
            //                // Sample output:
            //                //     Socket client sent message: "Hi friends 👋!<|EOM|>"
            //                //     Socket client received acknowledgment: "<|ACK|>"
            //            }

            //            client.Shutdown(SocketShutdown.Both);
            //        }
            //        catch (Exception exx)
            //        {
            //            Console.WriteLine(exx.Message);
            //        }

            //var web = new WsClient();
            //await web.ConnectAsync("wss://192.168.137.206:8090");
            //var sweb = new WebSocket4Net.WebSocket("ws://192.168.137.206:8090", "/ws");

            //sweb.Open();
            //sweb.Closed += (o, e) => Console.WriteLine("Closed.");
            //sweb.Opened += (o, e) => Console.WriteLine("Opened.");
            //sweb.MessageReceived += (o, e) =>
            //{
            //    Console.WriteLine(">>: " + e.Message);
            //};
            //sweb.Error += (o, e) => Console.WriteLine("ERR: " + e.Exception.Message);

            //Console.WriteLine("Enter a command, or 'q' to quit.");
            //var command = Console.ReadLine();

            //while (command != "q")
            //{
            //    if (command == "clear")
            //    {
            //        Console.Clear();
            //    }
            //    else
            //    {
            //        sweb.Send(command);
            //    }

            //    command = Console.ReadLine();
            //}

            //sweb.Close();
            //var m = new Mqtt();
            //m.Connect();

            // create client instance
            try
            {
                MqttClient client = new MqttClient("192.168.137.206", 8090, false, MqttSslProtocols.None, null, null);
                //MqttClient client = new MqttClient(IPAddress.Parse("192.168.137.206" ));
                //MqttClient client = new MqttClient("wss://192.168.137.206:8090/ws");

                // register to message received
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);

                // subscribe to the topic "/home/temperature" with QoS 2
                //client.Subscribe(new string[] { "/home/temperature" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                client.Publish("/app/chat.sendMessage", Encoding.ASCII.GetBytes("hello"));
            }
            catch (Exception exx)
            {
                Console.WriteLine(exx.Message);
            }

            //Process.GetCurrentProcess().WaitForExit();
            Console.WriteLine("Finished");
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
        }

        private static void Ws_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("server closed");
        }

        private static void Ws_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            Console.WriteLine("Server said: " + e.Message);
        }
        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine(Encoding.UTF8.GetString(args.Data));
        }

        static void ServerConnected(object sender, EventArgs args)
        {
            Console.WriteLine("Server connected");
        }

        static void ServerDisconnected(object sender, EventArgs args)
        {
            Console.WriteLine("Server disconnected");
        }
    }
}
