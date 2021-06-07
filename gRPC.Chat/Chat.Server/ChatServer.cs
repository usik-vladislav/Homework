using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Abstractions;
using Grpc.Core;

namespace Chat.Server
{
	public class ChatServer : IChat
	{
		public string Name { get; set; }

		private Grpc.Core.Server GrpcServer { get; }
		private ChatService ChatService { get; }
		private Action DisconnectHandle { get; }

		public ChatServer(int port, string name, Action<MessageViewModel> getMessageHandle, Action disconnectHandle)
		{
			DisconnectHandle = disconnectHandle;
			Name = name;
			ChatService = new ChatService(getMessageHandle);
			using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
			socket.Connect("8.8.8.8", 65530);
			IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
			GrpcServer = new Grpc.Core.Server
			{
				Services = { ChatRoom.BindService(ChatService) },
				Ports = { new ServerPort(endPoint?.Address.ToString(), port, ServerCredentials.Insecure) },
			};
			GrpcServer.Start();
        }

		public async Task SendMessageAsync(MessageViewModel messageViewModel)
		{
			var message = new Message
			{
				Author = messageViewModel.Author,
				Text = messageViewModel.Text,
				Time = messageViewModel.Time
			};
			await ChatService.SendMessage(message).ConfigureAwait(false);
		}

		public async Task DisconnectAsync()
		{
			var message = new Message
			{
				Author = "System",
				Text = "Abort",
				Time = DateTime.Now.ToShortTimeString()
			};
			await ChatService.SendMessage(message).ConfigureAwait(false);
			await GrpcServer.ShutdownAsync().ConfigureAwait(false);
			DisconnectHandle();
		}
	}
}
