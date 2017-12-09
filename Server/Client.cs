using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Common;
using System.Net;

namespace Server
{
    public class Client
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public EndPoint EndPoint{ get; set; }

        public Client(string name, EndPoint endpoint)
        {
            Id = new Guid();
            Name = name;
            EndPoint = endpoint;
        }
    }
}
