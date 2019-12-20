using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoClient
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                Console.Write("Введите ip: ");
                var ip = Console.ReadLine();

                Console.Write("Введите порт: ");
                var port = Console.ReadLine();

                var ep = new IPEndPoint(IPAddress.Parse(ip ?? throw new InvalidOperationException()),
                    int.Parse(port ?? throw new InvalidOperationException()));

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Console.WriteLine("Подключение...");
                socket.Connect(ep);

                while (true)
                {
                    Console.Write("Введите сообщение:");
                    var message = Console.ReadLine();
                    var data = Encoding.Unicode.GetBytes(message);

                    Console.WriteLine("Отправка...");
                    socket.Send(data);

                    data = new byte[256];
                    var builder = new StringBuilder();

                    do
                    {
                        var bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);

                    Console.WriteLine("Ответ сервера: " + builder);
                    Console.WriteLine("Нажмите энтер, чтобы выйти...");
                    Console.WriteLine("Нажмите любую другую клавишу, чтобы продолжить...");
                    var info = Console.ReadKey();
                    if (info.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
