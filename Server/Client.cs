using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        NetworkStream Stream { get; set; }

        public Client(string name, NetworkStream stream)
        {
            Name = name;
            Stream = stream;
        }

        public event EventHandler<ClientMessageEventArgs> MessageRecieved;

        public void OnClientMessageBroadcasted(object source, ClientMessageEventArgs eventArg)
        {
            if (eventArg.ClientId != Id)
            {
                using (var streamWriter = new StreamWriter(Stream))
                {
                    streamWriter.WriteLine(eventArg.ClientMessage);
                    streamWriter.Flush();
                }
            }
        }

        public void Listen()
        {
            var binaryReader = new BinaryReader(Stream);
            var buffer = new byte[64];
            var closeConnectionFlag = false;
            
            var stringBuilder = new StringBuilder();

            while (!closeConnectionFlag)
            {
                while (Stream.DataAvailable)
                {
                    binaryReader.Read(buffer, 0, buffer.Length);
                    stringBuilder.Append(Encoding.Unicode.GetString(buffer));
                }

                string clientMessage = stringBuilder.ToString();

                if (String.IsNullOrWhiteSpace(clientMessage))
                {
                    MessageRecieved.Invoke(this, new ClientMessageEventArgs(this.Id, clientMessage));
                }
            }

            binaryReader.Close();
        }
    }
}
