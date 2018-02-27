using System;

namespace Common
{
    /// <summary>
    /// Аргумент события получения нового сообщения от клиента
    /// </summary>
    public class ClientGetMessageEventArgs : EventArgs
    {
        public string ClientMessage { get; set; }

        public ClientGetMessageEventArgs(string clientMessage)
        {
            ClientMessage = clientMessage;
        }
    }
}
