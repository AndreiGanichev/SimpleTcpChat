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
                var clientName = ReadFromSocket(childSocket);
                var newClient = new Client(clientName);
                Clients.Add(newClient);
                WriteInSocket(childSocket, WellKnownStrings.AnswerIfConnected );
                Console.WriteLine($"Клиент {clientName} подключен к чату");
                childSocket.Close();
            }
        }
        public void OnClientMessageReceived(object source, ClientMessageEventArgs eventArg)
        {
            var sender = (Client)source;
            var broadcastMessage = $"{sender.Name}: {eventArg.ClientMessage}";
            ClientMessageBroadcasted.Invoke(this, eventArg);
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
    }
}
