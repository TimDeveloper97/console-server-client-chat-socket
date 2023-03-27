using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace Client1
{
    class Program
    {
        const string name2 = "[client2]";
        const string name1 = "[client1]";
        static async Task Main(string[] args)
        {
            WatsonWsClient client = new WatsonWsClient(new Uri("ws://127.0.0.1:8888"));

            client.ServerConnected += (s, e) =>
            {
                client.SendAsync(name1);
            };

            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;

            await client.StartAsync();
            //if (client.Connected)
            //    Console.WriteLine("client connected");
            //else
            //    Console.WriteLine("client not connected");

            while (true)
            {
                var message = Console.ReadLine();
                await client.SendAsync($"{name1}{name2}" + message);
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
