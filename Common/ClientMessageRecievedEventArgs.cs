using System;

namespace Common
{
    public class ClientMessageEventArgs : EventArgs
    {
        public Guid ClientId { get; set; }
        public string ClientMessage { get; set; }

        public ClientMessageEventArgs(Guid clientId, string clientMessage)
        {
            ClientId = clientId;
            ClientMessage = clientMessage;
        }
    }
}
