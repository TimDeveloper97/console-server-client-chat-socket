using Microsoft.AspNet.SignalR.Client;
using System;

namespace ConsoleApp2
{
    class Program
    {
        private static void Main(string[] args)
        {
            ////Set connection
            //var connection = new HubConnection("http://192.168.137.206:8090");
            ////Make proxy to hub based on hub name on server
            //var myHub = connection.CreateHubProxy("ws");
            ////Start connection

            //connection.Start().ContinueWith(task => {
            //    if (task.IsFaulted)
            //    {
            //        Console.WriteLine("There was an error opening the connection:{0}",
            //                          task.Exception.GetBaseException());
            //    }
            //    else
            //    {
            //        Console.WriteLine("Connected");
            //    }

            //}).Wait();

            //myHub.Invoke<string>("/app/chat.sendMessage", "HELLO World ").ContinueWith(task => {
            //    if (task.IsFaulted)
            //    {
            //        Console.WriteLine("There was an error calling send: {0}",
            //                          task.Exception.GetBaseException());
            //    }
            //    else
            //    {
            //        Console.WriteLine(task.Result);
            //    }
            //});

            //myHub.On<string>("/app/chat.sendMessage", param => {
            //    Console.WriteLine(param);
            //});

            //myHub.Invoke<string>("DoSomething", "I'm doing something!!!").Wait();


            //Console.Read();
            //connection.Stop();

            const int ADMIN = 1;

            var x = nameof(ADMIN);
            Console.WriteLine(x);
        }
    }
}

