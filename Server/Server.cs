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

        public void ConnectClients()
        {
            while(true)
            {
                Listener.Start();
                var stream = Listener.AcceptTcpClient().GetStream();
                StreamReader reader = new StreamReader(stream);
                var clientName = reader.ReadLine();
                var newClient = new Client(clientName, stream);
                newClient.MessageRecieved += OnClientMessageReceived;
                ClientMessageBroadcasted += newClient.OnClientMessageBroadcasted;
                Clients.Add(newClient);
                Console.WriteLine($"Клиент {clientName} подключен к чату");
            }
        }
        public void OnClientMessageReceived(object source, ClientMessageEventArgs eventArg)
        {
            var sender = (Client)source;
            var broadcastMessage = $"{sender.Name}: {eventArg.ClientMessage}";
            ClientMessageBroadcasted.Invoke(this, eventArg);
        }
    }
}
