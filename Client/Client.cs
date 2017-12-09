using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        private string _clientName;
        private TcpClient _tcpClient;
        private bool _isListening;

        public Client(string name, TcpClient client)
        {
            _clientName = name;
            _tcpClient = client;
        }

        public event EventHandler<ClientMessageEventArgs> ClientReceivedMessageEvent;

        public bool TryConnect()
        {
            var writer = new StreamWriter(_tcpClient.GetStream());
            var reader = new StreamReader(_tcpClient.GetStream());
            string serverAnswer = null;

            try
            {
                writer.WriteLine(_clientName);
                writer.Flush();
                serverAnswer = reader.ReadLine();
            }
            finally
            {
                writer.Close();
                reader.Close();
            }

            if (!string.IsNullOrWhiteSpace(serverAnswer)
                && !string.Equals(serverAnswer, WellKnownStrings.AnswerIfConnected, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            _isListening = true;
            StartListeningServer();
            return true;
        }

        public void SendMessage(string message)
        {
            using (var writer = new StreamWriter(_tcpClient.GetStream()))
            {
                writer.WriteLine(message);
                writer.Flush();
            }
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
                        using (var reader = new StreamReader(_tcpClient.GetStream()))
                        {
                            var readMessage = reader.ReadLine();
                            ClientReceivedMessageEvent.Invoke(this, new ClientMessageEventArgs(Guid.Empty, readMessage));
                        }
                    }
                });
        }
    }
}
