using Common;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private Socket _socket;

        public int Id { get; }
        public string Name { get; }
        public event EventHandler<ClientGetMessageEventArgs> ClientGetMessage;

        public Client(int id, string name, Socket socket)
        {
            Id = id;
            Name = name;
            _socket = socket;
        }

        public void StartListening()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var message = _socket.ReadMessage();
                        ClientGetMessage?.Invoke(this, new ClientGetMessageEventArgs(message));
                    }
                    catch (SocketException)
                    {
                        SendMessage("Истекло время ожидания");
                        break;
                    }
                }
            });
        }

        public void SendMessage(string message)
        {
            _socket.WriteMessage(message);
        }
    }
}
