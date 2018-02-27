using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var serverThread = new Thread(new ThreadStart(server.Start));
            serverThread.Start();
        }
    }
}
