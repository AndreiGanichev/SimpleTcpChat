using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите ваше имя");
            var clientName = Console.ReadLine();
            var tcpClient = new TcpClient("127.0.0.1", 8080);
            var writer = new StreamWriter(tcpClient.GetStream());
            var reader = new BinaryReader(tcpClient.GetStream());
            writer.WriteLine(clientName);
            writer.Flush();
            var buffer = new byte[256];
            reader.Read(buffer, 0, buffer.Length);

            if (!Encoding.Unicode.GetString(buffer).Equals("CONNECTED", StringComparison.Ordinal))
            {
                return;
            }

            Console.WriteLine($"Вы подключены к чату под именем {clientName}");
            Console.ReadKey();
        }
    }
}
