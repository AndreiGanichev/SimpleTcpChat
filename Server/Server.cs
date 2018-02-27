using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private int _idsCounter;
        private object _broadCastLock = new object();
        private IList<Client> _clients { get; set; }
        private EndPoint _clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 51570);
        private EndPoint _serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

        public Server()
        {
            _clients = new List<Client>();
            //Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        }

        public void Start()
        {
            //Listener.Start();
            //var parentSocket = Listener.AcceptSocket();//блокирует вызывающий поток, пока клиент не запросит соединение
            var parentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            parentSocket.Bind(_serverEndPoint);
            parentSocket.Listen(5);

            while (true)
            {
                var childSocket = parentSocket.Accept();//берем из очереди запросов один и создаем для него дочерний сокет
                var message = childSocket.ReadMessage();
                
                if (message.Contains(WellKnownStrings.ConnectionRequestCode))
                {
                    var clientName = message.Split(':')[1];
                    var newClient = new Client(++_idsCounter, clientName, childSocket);
                    newClient.ClientGetMessage += ClientGotMessage;
                    newClient.StartListening();
                    _clients.Add(newClient);
                    childSocket.WriteMessage($"{WellKnownStrings.AnswerIfConnected}{newClient.Id.ToString()}");
                    Console.WriteLine($"Клиент {clientName}, порт {childSocket.RemoteEndPoint.ToString()} подключен к чату");
                }          
            }
        }

        private void ClientGotMessage(object sender, ClientGetMessageEventArgs e)
        {
            lock(_broadCastLock)
            {
                var client = sender as Client;

                if (client != null)
                {
                    BroadcastMessage(e.ClientMessage, client.Id);
                }
            }            
        }

        /// <summary>
        /// Рассылает сообщения всем клиентам, кроме автора
        /// </summary>
        /// <param name="message"></param>
        /// <param name="authorId"></param>
        private void BroadcastMessage(string message, int authorId)
        {
            Parallel.ForEach(_clients.Where(c => c.Id != authorId),
                (c) =>
                {
                    c.SendMessage($"{c.Name}: {message}");
                });
        }
    }
}
