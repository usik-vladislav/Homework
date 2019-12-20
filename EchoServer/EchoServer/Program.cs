using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EchoServer
{
    internal class Program
    {
        private static void Main()
        {
            Console.Write("Введите ip: ");
            var ip = Console.ReadLine();

            Console.Write("Введите порт: ");
            var port = Console.ReadLine();

            var ep = new IPEndPoint(IPAddress.Parse(ip ?? throw new InvalidOperationException()),
                                int.Parse(port ?? throw new InvalidOperationException()));
            Console.WriteLine("Локальный адрес: {0}", ep);

            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(ep);
                serverSocket.Listen(15);

                while (true)
                {
                    var socket = serverSocket.Accept();
                    Console.WriteLine("Кто-то подключился...");
                    var clientHandler = new ClientHandler(socket);
                    var thread = new Thread(clientHandler.Process);
                    thread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
