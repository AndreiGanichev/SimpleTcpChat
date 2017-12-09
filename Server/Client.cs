using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Common;

namespace Server
{
    public class Client
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }

        public Client(string name)
        {
            Id = new Guid();
            Name = name;
        }
    }
}
