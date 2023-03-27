﻿using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace Client2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WatsonWsClient client = new WatsonWsClient(new Uri("ws://127.0.0.1:8888"));
            client.ServerConnected += ServerConnected;
            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;
            //client.KeepAliveInterval = 10000000;

            await client.StartAsync();
            if (client.Connected)
                Console.WriteLine("client connected");
            else
                Console.WriteLine("client not connected");

            //await client.SendAsync("hello server duyanh day");

            while (true)
            {
                
                var message = Console.ReadLine();
                await client.SendAsync("[client2][client1]" + message);
            }
            Process.GetCurrentProcess().WaitForExit();
        }

        static void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine("Message from server: " + Encoding.UTF8.GetString(args.Data));
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
