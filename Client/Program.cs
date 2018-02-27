using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите ваше имя");
            var clientName = Console.ReadLine();
            var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            var chatClient = new Client(clientName, serverEndPoint);
            chatClient.ClientGetMessageEvent += OnMessageReceived;

            if (chatClient.TryConnect())
            {
                Console.WriteLine($"Связь с сервером установлена");
            }
            else
            {
                Console.WriteLine($"Нет связи с сервером");
            }

            while (true)
            {
                Console.Write("Вы:");
                var message = Console.ReadLine();
                chatClient.SendMessage(message);
            }
        }

        private static void OnMessageReceived(object sender, ClientGetMessageEventArgs e)
        {
            Console.WriteLine(e.ClientMessage);
        }
    }
}
