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
        private Guid _clientId;
        private string _clientName;
        private EndPoint _serverEndPoint;
        private bool _isListening;

        public Client(string name, EndPoint serverEndPoint)
        {
            _clientName = name;
            _serverEndPoint = serverEndPoint;
        }

        public event EventHandler<ClientMessageEventArgs> ClientReceivedMessageEvent;

        public bool TryConnect()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_serverEndPoint);
            socket.WriteMessage($"{WellKnownStrings.ConnectionRequestCode}{_clientName}");
            var serverAnswerParts = socket.ReadMessage().Split(':');

            if (serverAnswerParts == null || serverAnswerParts.Length != 2
                && !string.Equals(serverAnswerParts[0], WellKnownStrings.AnswerIfConnected, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            _isListening = true;
            StartListeningServer();
            return true;
        }

        public void SendMessage(string message)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(_serverEndPoint);
            socket.WriteMessage($"{_clientId.ToString()}:{message}");
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
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(_serverEndPoint);
                        var readMessage = socket.ReadMessage();
                        ClientReceivedMessageEvent.Invoke(this, new ClientMessageEventArgs(Guid.Empty, readMessage));
                    }
                });
        }
    }
}
