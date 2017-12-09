using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
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
