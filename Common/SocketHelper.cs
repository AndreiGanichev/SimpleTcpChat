using System.Net.Sockets;
using System.Text;

namespace Common
{
    public static class SocketHelper
    {
        public static string ReadMessage(this Socket socket)
        {
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = socket.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (socket.Available > 0);

            return builder.ToString();
        }

        public static void WriteMessage(this Socket socket, string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);
        }
    }
}
