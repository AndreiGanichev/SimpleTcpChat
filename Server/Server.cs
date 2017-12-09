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
        public IList<Client> Clients { get; set; }
        public TcpListener Listener { get; set; }
        public event EventHandler<ClientMessageEventArgs> ClientMessageBroadcasted;

        public Server()
        {
            Clients = new List<Client>();
            Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        }

        public void StartConnectClients()
        {
            Listener.Start();
            var parentSocket = Listener.AcceptSocket();//блокирует вызывающий поток, пока клиент не запросит соединение

            while (true)
            {
                var childSocket = parentSocket.Accept();//берем из очереди запросов один и создаем для него дочерний сокет
                var message = ReadFromSocket(childSocket);
                
                if (message.Contains(WellKnownStrings.ConnectionRequestCode))
                {
                    var clientName = message.Replace(WellKnownStrings.ConnectionRequestCode, string.Empty);
                    var newClient = new Client(clientName, childSocket.RemoteEndPoint);
                    Clients.Add(newClient);
                    WriteInSocket(childSocket, $"{WellKnownStrings.AnswerIfConnected}{newClient.Id}");
                    Console.WriteLine($"Клиент {clientName} подключен к чату");
                }
                else
                {
                    var parts = message.Split(':');
                    var authorId = new Guid(parts[0]);
                    BroadcastMessage(parts[1], authorId);
                }

                childSocket.Shutdown(SocketShutdown.Both);
                childSocket.Close();
            }
        }

        private string ReadFromSocket(Socket socket)
        {
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = socket.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);

            return builder.ToString();
        }

        private void WriteInSocket(Socket socket, string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);
        }

        private void BroadcastMessage(string message, Guid authorId)
        {
            Parallel.ForEach(Clients.Where(c => c.Id != authorId),
                (c) =>
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(c.EndPoint);
                    WriteInSocket(socket, message);
                });
        }
    }
}
