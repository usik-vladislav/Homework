﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Abstractions;
using Grpc.Core;

namespace Chat.Server
{
	//Объект чата-сервера. Создает сервер через gRPC.
	public class ChatServer : IChat
	{
		public ChatServer(int port, string name, Action<MessageViewModel> getMessageHandle, Action disconnectHandle)
		{
			DisconnectHandle = disconnectHandle;
			Name = name;
			ChatService = new ChatService(getMessageHandle);
			using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
			socket.Connect("8.8.8.8", 65530);
			var endPoint = socket.LocalEndPoint as IPEndPoint;
			GrpcServer = new Grpc.Core.Server
			{
				Services = {ChatRoom.BindService(ChatService)},
				Ports = {new ServerPort(endPoint?.Address.ToString(), port, ServerCredentials.Insecure)}
			};
			GrpcServer.Start();
		}

		private Grpc.Core.Server GrpcServer { get; }
		private ChatService ChatService { get; }
		private Action DisconnectHandle { get; }
		public string Name { get; set; }

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