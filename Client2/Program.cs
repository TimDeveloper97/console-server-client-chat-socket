using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace Client2
{
    class Program
    {
        const string name2 = "[client2]";
        const string name1 = "[client1]";
        static async Task Main(string[] args)
        {
            WatsonWsClient client = new WatsonWsClient(new Uri("ws://192.168.137.206:8887/ws/chat"));
            //WatsonWsClient client = new WatsonWsClient("192.168.137.206", 8090, false);

            //WatsonWsClient client = new WatsonWsClient(new Uri("ws://192.168.137.206:8090"));

            client.ServerConnected += (s,e) =>
            {
                var mess = client.SendAsync(name2);
            };

            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;

            await client.StartAsync();
            if (client.Connected)
                Console.WriteLine("client connected");
            else
                Console.WriteLine("client not connected");

            while (true)
            {
                var message = Console.ReadLine();
                await client.SendAsync($"{name2}{name1}" + message);
            }
            Process.GetCurrentProcess().WaitForExit();
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
