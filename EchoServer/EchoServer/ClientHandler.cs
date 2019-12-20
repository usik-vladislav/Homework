using System;
using System.Net.Sockets;

namespace EchoServer
{
    internal class ClientHandler
    {
        private readonly Socket _socket;

        public ClientHandler(Socket socket)
        {
            _socket = socket;
        }

        public void Process()
        {
            try
            {
                while (true)
                {
                    var data = new byte[256];

                    _socket.Receive(data);
                    _socket.Send(data);
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054 || ex.ErrorCode == 10053)
                {
                    Console.WriteLine("Кто-то отключился...");
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
                else
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
    }
}
