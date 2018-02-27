using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace Client
{
    public class Client
    {
        private int _clientId;
        private string _clientName;
        private EndPoint _serverEndPoint;
        private bool _isListening;
        private Socket _socket;

        public Client(string name, EndPoint serverEndPoint)
        {
            _clientName = name;
            _serverEndPoint = serverEndPoint;
        }

        public event EventHandler<ClientGetMessageEventArgs> ClientGetMessageEvent;

        public bool TryConnect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_serverEndPoint);
            _socket.WriteMessage($"{WellKnownStrings.ConnectionRequestCode}{_clientName}");
            var serverAnswerParts = _socket.ReadMessage().Split(':');
            bool result = false;

            if (serverAnswerParts != null || serverAnswerParts.Length == 2
                && string.Equals(serverAnswerParts[0], WellKnownStrings.AnswerIfConnected, StringComparison.OrdinalIgnoreCase))
            {
                _clientId = Int32.Parse(serverAnswerParts[1]);
                result = true;
            }          

            _isListening = true;
            StartListeningServer();
            return result;
        }

        public void SendMessage(string message)
        {
            _socket.WriteMessage($"{_clientId.ToString()}:{message}");
        }

        public void StopListening()
        {
            _isListening = false;
        }

        private void StartListeningServer()
        {
            Task.Run(
                () =>
                {
                    while (_isListening)
                    {
                        var readMessage = _socket.ReadMessage();
                        ClientGetMessageEvent.Invoke(this, new ClientGetMessageEventArgs(readMessage));
                    }
                });
        }
    }
}
